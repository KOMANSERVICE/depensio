using System.ComponentModel.DataAnnotations;

namespace depensio.Domain.Models;

public class PlanFeature
{

    public PlanId PlanId { get; set; }
    public FeatureId FeatureId { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty; // ex: nombre max user, produit

    // Relations
    public virtual Plan Plan { get; set; } = new();
    public virtual Feature Feature { get; set; } = new();

}
