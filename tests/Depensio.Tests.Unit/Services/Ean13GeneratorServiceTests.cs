using System.Text.Json;
using BuildingBlocks.Exceptions;
using depensio.Application.DTOs;
using depensio.Application.Helpers;
using depensio.Application.Interfaces;
using depensio.Application.Models;
using depensio.Application.Services;
using depensio.Application.Data;
using depensio.Domain.Constants;
using depensio.Domain.Enums;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using depensio.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Depensio.Tests.Unit.Services;

public class Ean13GeneratorServiceTests
{
    [Fact]
    public async Task GenerateBarcodeAsync_ReturnsManualBarcode_WhenManualMode()
    {
        var boutiqueId = Guid.NewGuid();
        using var context = CreateDbContext();
        var (service, _) = CreateService(context, boutiqueId, BarcodeGenerationMode.Manual);

        const string manualBarcode = "6131234567895";

        var result = await service.GenerateBarcodeAsync(boutiqueId, manualBarcode);

        result.Should().Be(manualBarcode);
    }

    [Fact]
    public async Task GenerateBarcodeAsync_ThrowsBadRequest_WhenManualBarcodeMissing()
    {
        var boutiqueId = Guid.NewGuid();
        using var context = CreateDbContext();
        var (service, _) = CreateService(context, boutiqueId, BarcodeGenerationMode.Manual);

        var act = async () => await service.GenerateBarcodeAsync(boutiqueId, null);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GenerateBarcodeAsync_ThrowsBadRequest_WhenManualBarcodeAlreadyExists()
    {
        var boutiqueId = Guid.NewGuid();
        using var context = CreateDbContext();
        var (service, _) = CreateService(context, boutiqueId, BarcodeGenerationMode.Manual);

        const string manualBarcode = "6131234567895";

        SeedProduct(context, boutiqueId, manualBarcode);

        var act = async () => await service.GenerateBarcodeAsync(boutiqueId, manualBarcode);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task GenerateBarcodeAsync_GeneratesValidBarcode_WhenAutoMode()
    {
        var boutiqueId = Guid.NewGuid();
        using var context = CreateDbContext();
        var (service, _) = CreateService(context, boutiqueId, BarcodeGenerationMode.Auto);

        var result = await service.GenerateBarcodeAsync(boutiqueId);

        result.Should().MatchRegex(@"^\d{13}$");
        result.StartsWith("613").Should().BeTrue();
    }

    [Fact]
    public void GetBarcodeValue_ProducesValidEan13()
    {
        var boutiqueId = Guid.NewGuid();
        using var context = CreateDbContext();
        var (service, _) = CreateService(context, boutiqueId, BarcodeGenerationMode.Auto);

        var result = service.GetBarcodeValue();

        result.Should().MatchRegex(@"^\d{13}$");
        BoolHelper.IsValidEan13(result).Should().BeTrue();
    }

    private static (Ean13GeneratorService Service, Mock<IBoutiqueSettingService> SettingsMock) CreateService(
        IDepensioDbContext dbContext,
        Guid boutiqueId,
        BarcodeGenerationMode mode)
    {
        var settingsMock = new Mock<IBoutiqueSettingService>();
        settingsMock
            .Setup(s => s.GetSettingAsync(boutiqueId, BoutiqueSettingKeys.PRODUCT_KEY))
            .ReturnsAsync(CreateSetting(mode));

        var service = new Ean13GeneratorService(settingsMock.Object, dbContext);
        return (service, settingsMock);
    }

    private static SettingDTO CreateSetting(BarcodeGenerationMode mode)
    {
        var values = new List<BoutiqueValue>
        {
            new()
            {
                Id = BoutiqueSettingKeys.PRODUCT_BARCODE_GENERATION_MODE,
                Value = mode.ToString()
            }
        };

        return new SettingDTO
        {
            Key = BoutiqueSettingKeys.PRODUCT_KEY,
            Value = JsonSerializer.Serialize(values)
        };
    }

    private static DepensioDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<DepensioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DepensioDbContext(options);
    }

    private static void SeedProduct(DepensioDbContext context, Guid boutiqueId, string barcode)
    {
        context.Products.Add(new Product
        {
            Id = ProductId.Of(Guid.NewGuid()),
            Name = "Existing product",
            Barcode = barcode,
            Price = 10m,
            CostPrice = 5m,
            Stock = 1,
            BoutiqueId = BoutiqueId.Of(boutiqueId),
            PurchaseItems = new List<PurchaseItem>(),
            SaleItems = new List<SaleItem>(),
            ProductItems = new List<ProductItem>()
        });

        context.SaveChanges();
    }
}
