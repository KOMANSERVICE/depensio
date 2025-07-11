using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record SaleItemId
{
    public Guid Value { get; }
    private SaleItemId(Guid value) => Value = value;
    public static SaleItemId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("SaleItemId cannot be empty.");
        }
        return new SaleItemId(value);
    }
}

