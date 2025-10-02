using System.ComponentModel.DataAnnotations;

namespace depensio.Domain.Models;

public class Feature : Entity<FeatureId>
{
    public string Key { get; set; } = string.Empty; // ex: "MaxUsers", "MaxProducts"

    public virtual ICollection<PlanFeature> PlanFeatures { get; set; } = new HashSet<PlanFeature>();
}
