
using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record PlanFeatureId
{
    public Guid Value { get; }
    private PlanFeatureId(Guid value) => Value = value;
    public static PlanFeatureId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("PlanFeatureId cannot be empty.");
        }

        return new PlanFeatureId(value);
    }
}
