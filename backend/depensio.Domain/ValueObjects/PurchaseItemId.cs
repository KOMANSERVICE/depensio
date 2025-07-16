using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record PurchaseItemId
{
    public Guid Value { get; }
    private PurchaseItemId(Guid value) => Value = value;
    public static PurchaseItemId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("PurchaseItemId cannot be empty.");
        }
        return new PurchaseItemId(value);
    }


}
