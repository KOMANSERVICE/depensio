
using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record MenuId
{
    public Guid Value { get; }
    private MenuId(Guid value) => Value = value;
    public static MenuId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("MenuId cannot be empty.");
        }

        return new MenuId(value);
    }
}
