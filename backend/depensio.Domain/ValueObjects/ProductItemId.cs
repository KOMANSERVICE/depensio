using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record ProductItemId
{
    public Guid Value { get; }
    private ProductItemId(Guid value) => Value = value;
    public static ProductItemId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("ProductItemId cannot be empty.");
        }

        return new ProductItemId(value);
    }
}