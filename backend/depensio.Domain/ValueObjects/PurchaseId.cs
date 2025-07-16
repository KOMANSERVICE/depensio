using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record PurchaseId
{
    public Guid Value { get; }
    private PurchaseId(Guid value) => Value = value;
    public static PurchaseId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("PurchaseId cannot be empty.");
        }
        return new PurchaseId(value);
    }


}
