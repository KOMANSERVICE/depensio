using depensio.Application.DTOs;
using depensio.Application.Interfaces;
using depensio.Application.Models;
using depensio.Application.Services;
using depensio.Application.UseCases.Products.Commands.UpdateProductByBoutique;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Domain.Constants;
using depensio.Domain.Enums;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using depensio.Infrastructure.Data;
using FluentAssertions;
using IDR.Library.BuildingBlocks.Contexts.Services;
using IDR.Library.BuildingBlocks.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using System.Text.Json;
using Xunit;
using Xunit.Gherkin.Quick;
using Feature = Xunit.Gherkin.Quick.Feature;

namespace Depensio.Tests.Acceptance.Products;

[FeatureFile("./Features/Products/ProductUpdateManualBarcode.feature")]
public sealed class ProductUpdateManualBarcodeSpec : Feature
{
    private readonly DepensioDbContext _dbContext;
    private readonly Mock<IGenericRepository<Product>> _productRepository = new();
    private readonly Mock<IUserContextService> _userContextService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IBoutiqueSettingService> _boutiqueSettingService = new();
    private readonly SettingOptionService _settingOptionService;

    private readonly Guid _boutiqueId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();
    private readonly string _userId = Guid.NewGuid().ToString();

    private Product? _trackedProduct;
    private Product? _updatedProduct;
    private UpdateProductByBoutiqueResult? _result;

    private UpdateProductByBoutiqueHandler _handler;

    public ProductUpdateManualBarcodeSpec()
    {
        var options = new DbContextOptionsBuilder<DepensioDbContext>()
            .UseInMemoryDatabase($"ProductUpdateManualBarcodeSpec-{Guid.NewGuid()}")
            .Options;

        _dbContext = new DepensioDbContext(options);
        _settingOptionService = new SettingOptionService(_boutiqueSettingService.Object);

        _productRepository
            .Setup(repo => repo.UpdateData(It.IsAny<Product>()))
            .Callback<Product>((product) => _updatedProduct = product);

        _unitOfWork
            .Setup(u => u.SaveChangesDataAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _handler = new UpdateProductByBoutiqueHandler(
            _productRepository.Object,
            _dbContext,
            _userContextService.Object,
            _unitOfWork.Object,
            _settingOptionService);
    }

    [Given(@"un produit existant accessible par l'utilisateur")]
    public async Task GivenExistingProduct()
    {
        _userContextService
            .Setup(s => s.GetUserId())
            .Returns(_userId);

        var product = new Product
        {
            Id = ProductId.Of(_productId),
            Name = "Produit existant",
            Barcode = "6130000000001",
            Price = 10m,
            CostPrice = 5m,
            Stock = 3,
            BoutiqueId = BoutiqueId.Of(_boutiqueId),
            ProductItems = new List<ProductItem>()
        };

        _trackedProduct = product;

        _productRepository
            .Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
            .Returns<Expression<Func<Product, bool>>, CancellationToken>((predicate, _) =>
            {
                var matches = predicate.Compile()(_trackedProduct!);
                return Task.FromResult(matches ? _trackedProduct! : null!);
            });

        var userBoutique = new UsersBoutique
        {
            Id = UsersBoutiqueId.Of(Guid.NewGuid()),
            UserId = _userId,
            BoutiqueId = BoutiqueId.Of(_boutiqueId),
            ProfileId = ProfileId.Of(Guid.NewGuid())
        };

        var boutique = new Boutique
        {
            Id = BoutiqueId.Of(_boutiqueId),
            Name = "Boutique Test",
            OwnerId = _userId,
            UsersBoutiques = new List<UsersBoutique> { userBoutique },
            Products = new List<Product> { product },
            Profiles = new List<Profile>(),
            StockLocations = new List<StockLocation>(),
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
    }

    [Given(@"la configuration de la boutique exige un code barre manuel")]
    public void GivenManualBarcodeConfiguration()
    {
        var settings = new List<BoutiqueValue>
        {
            new()
            {
                Id = BoutiqueSettingKeys.PRODUCT_BARCODE_GENERATION_MODE,
                Value = BarcodeGenerationMode.Manual.ToString()
            },
            new()
            {
                Id = BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE,
                Value = false
            }
        };

        var json = JsonSerializer.Serialize(settings);

        _boutiqueSettingService
            .Setup(s => s.GetSettingAsync(_boutiqueId, BoutiqueSettingKeys.PRODUCT_KEY))
            .ReturnsAsync(new SettingDTO
            {
                BoutiqueId = _boutiqueId,
                Key = BoutiqueSettingKeys.PRODUCT_KEY,
                Value = json
            });
    }

    [When(@"je mets a jour le produit avec le code barre ""(.*)""")]
    public async Task WhenIUpdateTheProduct(string manualBarcode)
    {
        var command = new UpdateProductByBoutiqueCommand(
            new ProductUpdateDTO(
                _productId,
                _boutiqueId,
                "Produit mis a jour",
                12m,
                6m,
                8,
                manualBarcode));

        _result = await _handler.Handle(command, CancellationToken.None);
    }

    [Then(@"le produit est mis a jour avec ce code barre")]
    public void ThenProductUpdated()
    {
        _result.Should().NotBeNull();
        _result!.Id.Should().Be(_productId);

        _updatedProduct.Should().NotBeNull();
        _updatedProduct!.Barcode.Should().Be("6131234567895");
        _updatedProduct.Name.Should().Be("Produit mis a jour");
        _updatedProduct.Stock.Should().Be(8);

        _unitOfWork.Verify(u => u.SaveChangesDataAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
