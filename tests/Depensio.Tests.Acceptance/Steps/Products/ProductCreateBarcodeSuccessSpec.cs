using depensio.Application.Interfaces;
using depensio.Application.UseCases.Products.Commands.CreateCodeBarre;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Domain.Enums;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using depensio.Infrastructure.Data;
using FluentAssertions;
using IDR.Library.BuildingBlocks.Contexts.Services;
using IDR.Library.BuildingBlocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Gherkin.Quick;
using Feature = Xunit.Gherkin.Quick.Feature;

namespace Depensio.Tests.Acceptance.Products;

[FeatureFile("./Features/Products/ProductCreateBarcodeSuccess.feature")]
public sealed class ProductCreateBarcodeSuccessSpec : Feature
{
    private readonly DepensioDbContext _dbContext;
    private readonly Mock<IGenericRepository<ProductItem>> _productItemRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IBarcodeService> _barcodeService;
    private readonly Mock<IUserContextService> _userContextService;
    private readonly Mock<IProductService> _productService;
    private readonly List<ProductItem> _persistedItems = new();
    private readonly Guid _boutiqueId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();
    private readonly string _userId = Guid.NewGuid().ToString();

    private CreateCodeBarreHandler _handler;
    private CreateCodeBarreResult? _result;

    public ProductCreateBarcodeSuccessSpec()
    {
        var options = new DbContextOptionsBuilder<DepensioDbContext>()
            .UseInMemoryDatabase($"ProductCreateBarcodeSuccessSpec-{Guid.NewGuid()}")
            .Options;

        _dbContext = new DepensioDbContext(options);
        _productItemRepository = new Mock<IGenericRepository<ProductItem>>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _barcodeService = new Mock<IBarcodeService>();
        _userContextService = new Mock<IUserContextService>();
        _productService = new Mock<IProductService>();

        _productItemRepository
            .Setup(repo => repo.AddRangeDataAsync(It.IsAny<IEnumerable<ProductItem>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<ProductItem>, CancellationToken>((items, _) => _persistedItems.AddRange(items))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(u => u.SaveChangesDataAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _handler = new CreateCodeBarreHandler(
            _dbContext,
            _productItemRepository.Object,
            _unitOfWork.Object,
            _barcodeService.Object,
            _userContextService.Object,
            _productService.Object);
    }

    [Given(@"un produit existant avec un stock de (.*) et un utilisateur autorise")]
    public async Task GivenProductWithStock(int stock)
    {
        _userContextService
            .Setup(s => s.GetUserId())
            .Returns(_userId);

        var product = new Product
        {
            Id = ProductId.Of(_productId),
            Name = "Produit Test",
            Barcode = "6130000000001",
            Price = 10m,
            CostPrice = 5m,
            Stock = stock,
            BoutiqueId = BoutiqueId.Of(_boutiqueId),
            ProductItems = new List<ProductItem>()
        };

        var userBoutique = new UsersBoutique
        {
            Id = UsersBoutiqueId.Of(Guid.NewGuid()),
            UserId = _userId,
            BoutiqueId = BoutiqueId.Of(_boutiqueId),
            Boutique = null!,
            ProfileId = ProfileId.Of(Guid.NewGuid())
        };

        var boutique = new Boutique
        {
            Id = BoutiqueId.Of(_boutiqueId),
            Name = "Boutique Test",
            OwnerId = _userId,
            PublicLink = "public",
            IsPublic = true,
            UsersBoutiques = new List<UsersBoutique> { userBoutique },
            Products = new List<Product> { product },
            Profiles = new List<Profile>(),
            BoutiqueSettings = new List<BoutiqueSetting>(),
            Sales = new List<Sale>(),
            Subscriptions = new List<Subscription>(),
            Purchases = new List<Purchase>()
        };

        userBoutique.Boutique = boutique;
        product.Boutique = boutique;

        await _dbContext.Boutiques.AddAsync(boutique);
        await _dbContext.Products.AddAsync(product);
        await _dbContext.UsersBoutiques.AddAsync(userBoutique);
        await _dbContext.SaveChangesAsync();

        _productService
            .Setup(s => s.GetOneProductAsync(_boutiqueId, _productId))
            .ReturnsAsync(product);
    }

    [Given(@"des codes barres existants pour ce produit")]
    public async Task GivenExistingBarcodes()
    {
        var existingItem = new ProductItem
        {
            Id = ProductItemId.Of(Guid.NewGuid()),
            ProductId = ProductId.Of(_productId),
            Barcode = "6130000000002",
            Status = ProductStatus.Available
        };

        await _dbContext.ProductItems.AddAsync(existingItem);
        await _dbContext.SaveChangesAsync();

        _barcodeService
            .SetupSequence(s => s.GetBarcodeValue())
            .Returns("6130000000003")
            .Returns("6130000000004");
    }

    [When(@"je genere (.*) nouveaux codes barres pour ce produit")]
    public async Task WhenIGenerateBarcodes(int count)
    {
        var command = new CreateCodeBarreCommand(
            new ProductItemDTO(
                Guid.NewGuid(),
                _boutiqueId,
                _productId,
                count,
                0,
                DiscountType.Amount));

        _result = await _handler.Handle(command, CancellationToken.None);
    }

    [Then(@"(.*) nouveaux codes barres uniques sont persistes")]
    public void ThenBarcodesArePersisted(int expectedCount)
    {
        _productItemRepository.Verify(
            repo => repo.AddRangeDataAsync(
                It.Is<IEnumerable<ProductItem>>(items =>
                    items.Count() == expectedCount &&
                    items.Select(i => i.Barcode).Distinct().Count() == expectedCount),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _persistedItems.Should().HaveCount(expectedCount);
    }

    [Then(@"la reponse contient les memes codes barres")]
    public void ThenResponseContainsBarcodes()
    {
        _result.Should().NotBeNull();
        _result!.Barcode.ProductId.Should().Be(_productId);
        _result.Barcode.Barcodes.Should().BeEquivalentTo(_persistedItems.Select(p => p.Barcode));

        _unitOfWork.Verify(u => u.SaveChangesDataAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
