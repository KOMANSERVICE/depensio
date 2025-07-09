using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;
public record UsersBoutiqueId
{
    public Guid Value { get; }
    private UsersBoutiqueId(Guid value) => Value = value;
    public static UsersBoutiqueId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("UsersBoutiqueId cannot be empty.");
        }

        return new UsersBoutiqueId(value);
    }
}

