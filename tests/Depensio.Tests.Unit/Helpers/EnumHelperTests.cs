using depensio.Application.Helpers;
using depensio.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Depensio.Tests.Unit.Helpers;

public class EnumHelperTests
{
    [Fact]
    public void ParseOrDefault_ShouldParseStringIgnoringCase()
    {
        var result = EnumHelper.ParseOrDefault("manual", BarcodeGenerationMode.Auto);

        result.Should().Be(BarcodeGenerationMode.Manual);
    }

    [Fact]
    public void ParseOrDefault_ShouldReturnEnumValue_WhenValueIsEnum()
    {
        var result = EnumHelper.ParseOrDefault(BarcodeGenerationMode.Hybrid, BarcodeGenerationMode.Auto);

        result.Should().Be(BarcodeGenerationMode.Hybrid);
    }

    [Fact]
    public void ParseOrDefault_ShouldReturnDefault_WhenValueCannotBeParsed()
    {
        var result = EnumHelper.ParseOrDefault("unknown", BarcodeGenerationMode.Auto);

        result.Should().Be(BarcodeGenerationMode.Auto);
    }
}
