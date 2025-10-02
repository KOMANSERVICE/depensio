using depensio.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace depensio.Domain.Models;

public class Plan : Entity<PlanId>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; } = 0;
    public BillingCycleStatus BillingCycle { get; set; } = BillingCycleStatus.None; // NONE, MONTHLY, YEARLY
    public bool RequiresPayment { get; set; } = false;
    public bool IsPopular { get; set; } = false;
    public bool IsDisplay { get; set; } = true;


    public virtual ICollection<PlanFeature> PlanFeatures { get; set; } = new HashSet<PlanFeature>();
    public virtual ICollection<PlanMenu> PlanMenus { get; set; } = new HashSet<PlanMenu>();
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new HashSet<Subscription>();
}
