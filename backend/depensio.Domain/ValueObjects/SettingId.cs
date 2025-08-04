using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record SettingId
{
    public Guid Value { get; }
    private SettingId(Guid value) => Value = value;
    public static SettingId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("SettingId cannot be empty.");
        }
        return new SettingId(value);
    }


}
