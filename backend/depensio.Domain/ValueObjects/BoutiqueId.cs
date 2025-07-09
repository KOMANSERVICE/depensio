
using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;
public record BoutiqueId
{
    public Guid Value { get; }
    private BoutiqueId(Guid value) => Value = value;
    public static BoutiqueId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("BoutiqueId cannot be empty.");
        }

        return new BoutiqueId(value);
    }
}
