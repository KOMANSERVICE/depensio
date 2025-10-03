
using depensio.Domain.Exceptions;

namespace depensio.Domain.ValueObjects;

public record PlanMenuId
{
    public Guid Value { get; }
    private PlanMenuId(Guid value) => Value = value;
    public static PlanMenuId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("PlanMenuId cannot be empty.");
        }

        return new PlanMenuId(value);
    }
}
