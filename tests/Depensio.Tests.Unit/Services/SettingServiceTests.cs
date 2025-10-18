using System.Text.Json;
using depensio.Application.Models;
using depensio.Application.Services;
using depensio.Domain.Constants;
using FluentAssertions;
using Xunit;

namespace Depensio.Tests.Unit.Services;

public class SettingServiceTests
{
    [Fact]
    public void GetSetting_ShouldReturnPresetConfiguration_ForKnownKey()
    {
        var service = new SettingService();

        var result = service.GetSetting(BoutiqueSettingKeys.PRODUCT_KEY);

        result.Key.Should().Be(BoutiqueSettingKeys.PRODUCT_KEY);
        result.Value.Should().NotBeNullOrWhiteSpace();

        var values = JsonSerializer.Deserialize<List<BoutiqueValue>>(result.Value);
        values.Should().NotBeNull();
        values!
            .Select(v => v.Id)
            .Should()
            .Contain(BoutiqueSettingKeys.PRODUCT_BARCODE_GENERATION_MODE);
    }

    [Fact]
    public void GetSetting_ShouldReturnEmptyDto_ForUnknownKey()
    {
        var service = new SettingService();

        var result = service.GetSetting("unknown.key");

        result.Key.Should().BeEmpty();
        result.Value.Should().BeEmpty();
    }
}
