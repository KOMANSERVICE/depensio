namespace depensio.Domain.Models;

public class Subscription : Entity<Guid>
{
    public Guid BoutiqueId { get; set; }
    public Guid PlanId { get; set; }
    public string Month { get; set; }
    public string Status { get; set; }
    public string Source { get; set; }
    public decimal Bonus { get; set; }

    public Boutique Boutique { get; set; }
    public Plan Plan { get; set; }
}
