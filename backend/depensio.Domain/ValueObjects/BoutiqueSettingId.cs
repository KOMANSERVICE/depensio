
using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;
public record BoutiqueSettingId
{
    public Guid Value { get; }
    private BoutiqueSettingId(Guid value) => Value = value;
    public static BoutiqueSettingId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("BoutiqueSettingId cannot be empty.");
        }

        return new BoutiqueSettingId(value);
    }
}
