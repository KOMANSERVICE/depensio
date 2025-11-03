using depensio.Application.Interfaces;
using depensio.Application.UseCases.Products.Commands.CreateCodeBarre;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Domain.Enums;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using depensio.Infrastructure.Data;
using FluentAssertions;
using IDR.Library.BuildingBlocks.Contexts.Services;
using IDR.Library.BuildingBlocks.Exceptions;
using IDR.Library.BuildingBlocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Gherkin.Quick;
using Feature = Xunit.Gherkin.Quick.Feature;

namespace Depensio.Tests.Acceptance.Products;

[FeatureFile("./Features/Products/ProductCreateBarcodeUnauthorized.feature")]
public sealed class ProductCreateBarcodeUnauthorizedSpec : Feature
{
    private readonly DepensioDbContext _dbContext;
    private readonly Mock<IGenericRepository<ProductItem>> _productItemRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IBarcodeService> _barcodeService = new();
    private readonly Mock<IUserContextService> _userContextService = new();
    private readonly Mock<IProductService> _productService = new();

    private readonly Guid _boutiqueId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();
    private readonly string _userId = Guid.NewGuid().ToString();
    private Exception? _exception;

    private CreateCodeBarreHandler _handler;

    public ProductCreateBarcodeUnauthorizedSpec()
    {
        var options = new DbContextOptionsBuilder<DepensioDbContext>()
            .UseInMemoryDatabase($"ProductCreateBarcodeUnauthorizedSpec-{Guid.NewGuid()}")
            .Options;

        _dbContext = new DepensioDbContext(options);
        _handler = new CreateCodeBarreHandler(
            _dbContext,
            _productItemRepository.Object,
            _unitOfWork.Object,
            _barcodeService.Object,
            _userContextService.Object,
            _productService.Object);
    }

    [Given(@"un produit existant et un utilisateur non autorise")]
    public async Task GivenUnauthorizedUser()
    {
        _userContextService
            .Setup(s => s.GetUserId())
            .Returns(_userId);

        var otherUser = Guid.NewGuid().ToString();

        var product = new Product
        {
            Id = ProductId.Of(_productId),
            Name = "Produit Test",
            Barcode = "6130000000001",
            Price = 10m,
            CostPrice = 5m,
            Stock = 5,
            BoutiqueId = BoutiqueId.Of(_boutiqueId),
            ProductItems = new List<ProductItem>()
        };

        var boutique = new Boutique
        {
            Id = BoutiqueId.Of(_boutiqueId),
            Name = "Boutique Test",
            OwnerId = otherUser,
            UsersBoutiques = new List<UsersBoutique>
            {
                new()
                {
                    Id = UsersBoutiqueId.Of(Guid.NewGuid()),
                    UserId = otherUser,
                    BoutiqueId = BoutiqueId.Of(_boutiqueId),
                    ProfileId = ProfileId.Of(Guid.NewGuid())
                }
            },
            Products = new List<Product> { product },
            Profiles = new List<Profile>(),
            StockLocations = new List<StockLocation>(),
            BoutiqueSettings = new List<BoutiqueSetting>(),
            Subscriptions = new List<Subscription>(),
            Sales = new List<Sale>(),
            Purchases = new List<Purchase>()
        };

        product.Boutique = boutique;

        await _dbContext.Boutiques.AddAsync(boutique);
        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        _productService
            .Setup(s => s.GetOneProductAsync(_boutiqueId, _productId))
            .ReturnsAsync(product);
    }

    [When(@"je tente de generer (.*) code barre pour ce produit")]
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

        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"une erreur d'autorisation est renvoyee")]
    public void ThenUnauthorizedError()
    {
        _exception.Should().NotBeNull();
        _exception.Should().BeOfType<UnauthorizedException>();

        _productItemRepository.Verify(
            repo => repo.AddRangeDataAsync(It.IsAny<IEnumerable<ProductItem>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
