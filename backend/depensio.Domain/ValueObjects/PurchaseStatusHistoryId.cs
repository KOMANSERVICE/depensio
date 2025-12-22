using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record PurchaseStatusHistoryId
{
    public Guid Value { get; }
    private PurchaseStatusHistoryId(Guid value) => Value = value;
    public static PurchaseStatusHistoryId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("PurchaseStatusHistoryId cannot be empty.");
        }
        return new PurchaseStatusHistoryId(value);
    }
}
