using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record SaleStatusHistoryId
{
    public Guid Value { get; }
    private SaleStatusHistoryId(Guid value) => Value = value;
    public static SaleStatusHistoryId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("SaleStatusHistoryId cannot be empty.");
        }
        return new SaleStatusHistoryId(value);
    }
}
