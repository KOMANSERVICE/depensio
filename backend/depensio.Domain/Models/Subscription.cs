using depensio.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace depensio.Domain.Models;

public class Subscription : Entity<SubscriptionId>
{
    public PlanId PlanId { get; set; }
    public BoutiqueId BoutiqueId { get; set; }

    public int Month { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active; // active, expired, canceled
    public string Source { get; set; } = string.Empty;
    public string Bonus { get; set; } = string.Empty;


    // Relations
    public virtual Plan Plan { get; set; } = new Plan();
    public virtual Boutique Boutique { get; set; } = new Boutique();
}
