
using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record SubscriptionId
{
    public Guid Value { get; }
    private SubscriptionId(Guid value) => Value = value;
    public static SubscriptionId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("SubscriptionId cannot be empty.");
        }

        return new SubscriptionId(value);
    }
}
