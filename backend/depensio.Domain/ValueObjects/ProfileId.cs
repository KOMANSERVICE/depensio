
using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record ProfileId
{
    public Guid Value { get; }
    private ProfileId(Guid value) => Value = value;
    public static ProfileId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("ProfileId cannot be empty.");
        }

        return new ProfileId(value);
    }
}
