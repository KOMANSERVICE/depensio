
using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record ProfileMenuId
{
    public Guid Value { get; }
    private ProfileMenuId(Guid value) => Value = value;
    public static ProfileMenuId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("ProfileMenuId cannot be empty.");
        }

        return new ProfileMenuId(value);
    }
}
