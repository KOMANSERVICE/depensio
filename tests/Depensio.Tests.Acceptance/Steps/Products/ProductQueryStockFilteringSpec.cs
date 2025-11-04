using depensio.Application.DTOs;
using depensio.Application.Interfaces;
using depensio.Application.Models;
using depensio.Application.UseCases.Products.Queries.GetProductByBoutiqueWithStockSetting;
using depensio.Domain.Constants;
using depensio.Domain.Enums;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using System.Linq;
using System.Text.Json;
using Xunit;
using Xunit.Gherkin.Quick;
using Feature = Xunit.Gherkin.Quick.Feature;

namespace Depensio.Tests.Acceptance.Products;

[FeatureFile("./Features/Products/ProductQueryStockFiltering.feature")]
public sealed class ProductQueryStockFilteringSpec : Feature
{
    private readonly Mock<IBoutiqueSettingService> _settingService = new();
    private readonly Mock<IProductService> _productService = new();
    private readonly Guid _boutiqueId = Guid.NewGuid();

    private GetProductByBoutiqueWithStockSettingHandler _handler;
    private GetProductByBoutiqueWithStockSettingResult? _result;

    public ProductQueryStockFilteringSpec()
    {
        _handler = new GetProductByBoutiqueWithStockSettingHandler(
            _settingService.Object,
            _productService.Object);
    }

    [Given(@"une boutique qui interdit la vente avec un stock nul")]
    public void GivenZeroStockForbidden()
    {
        var settings = new List<BoutiqueValue>
        {
            new()
            {
                Id = BoutiqueSettingKeys.VENTE_AUTORISER_VENTE_AVEC_STOCK_ZERO,
                Value = false
            }
        };

        var json = JsonSerializer.Serialize(settings);

        _settingService
            .Setup(s => s.GetSettingAsync(_boutiqueId, BoutiqueSettingKeys.VENTE_KEY))
            .ReturnsAsync(new SettingDTO
            {
                BoutiqueId = _boutiqueId,
                Key = BoutiqueSettingKeys.VENTE_KEY,
                Value = json
            });
    }

    [Given(@"plusieurs produits dont certains ont un stock nul")]
    public void GivenProducts()
    {
        var productWithZeroStock = new Product
        {
            Id = ProductId.Of(Guid.NewGuid()),
            Name = "Produit B",
            Barcode = "6130000000002",
            Price = 15m,
            CostPrice = 8m,
            Stock = 0,
            BoutiqueId = BoutiqueId.Of(_boutiqueId),
            ProductItems = new List<ProductItem>()
        };

        var productWithStock = new Product
        {
            Id = ProductId.Of(Guid.NewGuid()),
            Name = "Produit A",
            Barcode = "6130000000004",
            Price = 20m,
            CostPrice = 10m,
            Stock = 4,
            BoutiqueId = BoutiqueId.Of(_boutiqueId),
            ProductItems = new List<ProductItem>
            {
                new()
                {
                    Id = ProductItemId.Of(Guid.NewGuid()),
                    ProductId = ProductId.Of(Guid.NewGuid()),
                    Barcode = "6130000000005",
                    Status = ProductStatus.Available
                }
            }
        };

        productWithStock.ProductItems.Single().ProductId = productWithStock.Id;

        _productService
            .Setup(s => s.GetProductsAsync(_boutiqueId))
            .ReturnsAsync(new List<Product> { productWithZeroStock, productWithStock });
    }

    [When(@"je recupere les produits avec la configuration de stock")]
    public async Task WhenIQueryProducts()
    {
        var query = new GetProductByBoutiqueWithStockSettingQuery(_boutiqueId);

        _result = await _handler.Handle(query, CancellationToken.None);
    }

    [Then(@"seuls les produits avec du stock sont retournes")]
    public void ThenOnlyStockedProductsReturned()
    {
        _result.Should().NotBeNull();
        var products = _result!.Products.ToList();

        products.Should().HaveCount(1);
        products[0].Name.Should().Be("Produit A");
        products[0].Stock.Should().BeGreaterThan(0);
    }
}
