using depensio.Application.Helpers;
using FluentAssertions;
using Xunit;

namespace Depensio.Tests.Unit.Helpers;

public class BoolHelperTests
{
    [Theory]
    [InlineData("true", true)]
    [InlineData("False", false)]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("yes", true)]
    [InlineData("No", false)]
    [InlineData("Y", true)]
    [InlineData("n", false)]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(5, true)]
    [InlineData(0L, false)]
    [InlineData(2L, true)]
    [InlineData(0d, false)]
    [InlineData(0.5d, true)]
    [InlineData(null, false)]
    public void ToBool_ShouldConvertVariousInputs(object? input, bool expected)
    {
        BoolHelper.ToBool(input).Should().Be(expected);
    }

    [Fact]
    public void ToBool_ShouldRespectDefaultValue_ForUnknownType()
    {
        var unknown = new { Foo = "bar" };

        BoolHelper.ToBool(unknown).Should().BeFalse();
        BoolHelper.ToBool(unknown, defaultValue: true).Should().BeTrue();
    }

    [Theory]
    [InlineData("4006381333931", true)]
    [InlineData("5901234123457", true)]
    [InlineData("4006381333932", false)]
    [InlineData("123456789012", false)]
    [InlineData("12345678901234", false)]
    [InlineData("ABCDEFGHIJKLM", false)]
    [InlineData("", false)]
    [InlineData("            ", false)]
    public void IsValidEan13_ShouldValidateChecksumAndFormat(string barcode, bool expected)
    {
        BoolHelper.IsValidEan13(barcode).Should().Be(expected);
    }
}
