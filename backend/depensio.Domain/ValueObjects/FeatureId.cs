
using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record FeatureId
{
    public Guid Value { get; }
    private FeatureId(Guid value) => Value = value;
    public static FeatureId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("FeatureId cannot be empty.");
        }

        return new FeatureId(value);
    }
}
