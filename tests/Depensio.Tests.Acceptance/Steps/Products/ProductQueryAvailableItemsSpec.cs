using depensio.Application.Interfaces;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Application.UseCases.Products.Queries.GetProductItemByBoutique;
using depensio.Domain.Enums;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using depensio.Infrastructure.Data;
using FluentAssertions;
using IDR.Library.BuildingBlocks.Contexts.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Gherkin.Quick;
using Feature = Xunit.Gherkin.Quick.Feature;

namespace Depensio.Tests.Acceptance.Products;

[FeatureFile("./Features/Products/ProductQueryAvailableItems.feature")]
public sealed class ProductQueryAvailableItemsSpec : Feature
{
    private readonly DepensioDbContext _dbContext;
    private readonly Mock<IUserContextService> _userContextService = new();
    private readonly Guid _boutiqueId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();
    private readonly string _userId = Guid.NewGuid().ToString();

    private GetProductItemByBoutiqueHandler _handler;
    private GetProductItemByBoutiqueResult? _result;

    public ProductQueryAvailableItemsSpec()
    {
        var options = new DbContextOptionsBuilder<DepensioDbContext>()
            .UseInMemoryDatabase($"ProductQueryAvailableItemsSpec-{Guid.NewGuid()}")
            .Options;

        _dbContext = new DepensioDbContext(options);

        _handler = new GetProductItemByBoutiqueHandler(
            _dbContext,
            _userContextService.Object);
    }

    [Given(@"une boutique avec des items disponibles et non disponibles pour l'utilisateur")]
    public async Task GivenBoutiqueWithItems()
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
            Stock = 3,
            BoutiqueId = BoutiqueId.Of(_boutiqueId),
            ProductItems = new List<ProductItem>()
        };

        var availableItem = new ProductItem
        {
            Id = ProductItemId.Of(Guid.NewGuid()),
            ProductId = product.Id,
            Barcode = "6130000000002",
            Status = ProductStatus.Available,
            Product = product
        };

        var soldItem = new ProductItem
        {
            Id = ProductItemId.Of(Guid.NewGuid()),
            ProductId = product.Id,
            Barcode = "6130000000003",
            Status = ProductStatus.Sold,
            Product = product
        };

        product.ProductItems = new List<ProductItem> { availableItem, soldItem };

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
            BoutiqueSettings = new List<BoutiqueSetting>(),
            Subscriptions = new List<Subscription>(),
            Sales = new List<Sale>(),
            Purchases = new List<Purchase>()
        };

        userBoutique.Boutique = boutique;
        product.Boutique = boutique;

        await _dbContext.Boutiques.AddAsync(boutique);
        await _dbContext.UsersBoutiques.AddAsync(userBoutique);
        await _dbContext.Products.AddAsync(product);
        await _dbContext.ProductItems.AddRangeAsync(availableItem, soldItem);
        await _dbContext.SaveChangesAsync();
    }

    [When(@"je recupere les items de produit par boutique")]
    public async Task WhenIQueryProductItems()
    {
        var query = new GetProductItemByBoutiqueQuery(_boutiqueId);

        _result = await _handler.Handle(query, CancellationToken.None);
    }

    [Then(@"seuls les items disponibles sont retournes avec leur produit")]
    public void ThenOnlyAvailableItemsAreReturned()
    {
        _result.Should().NotBeNull();
        var groups = _result!.ProductBarcodes.ToList();

        groups.Should().HaveCount(1);

        var group = groups.Single();
        group.ProductId.Should().Be(_productId);
        group.ProductItems.Should().ContainSingle(item => item.Barcode == "6130000000002");
        group.ProductItems.Should().NotContain(item => item.Barcode == "6130000000003");
    }
}
