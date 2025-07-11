using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record SaleId
{
    public Guid Value { get; }
    private SaleId(Guid value) => Value = value;
    public static SaleId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("SaleId cannot be empty.");
        }
        return new SaleId(value);
    }


}
