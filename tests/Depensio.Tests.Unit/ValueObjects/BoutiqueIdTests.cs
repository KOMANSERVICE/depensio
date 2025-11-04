using depensio.Domain.Exceptions;
using depensio.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Depensio.Tests.Unit.ValueObjects;

public class BoutiqueIdTests
{
    [Fact]
    public void Of_ShouldCreateInstance_WhenGuidIsValid()
    {
        var guid = Guid.NewGuid();

        var result = BoutiqueId.Of(guid);

        result.Value.Should().Be(guid);
    }

    [Fact]
    public void Of_ShouldThrowDomainException_WhenGuidIsEmpty()
    {
        var act = () => BoutiqueId.Of(Guid.Empty);

        act.Should().Throw<DomainException>();
    }
}
