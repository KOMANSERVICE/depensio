namespace depensio.Domain.Models;

public class LoyaltyReward : Entity<Guid>
{
    public Guid Id { get; set; }
    public Guid BoutiqueId { get; set; }
    public string Type { get; set; }
    public DateTime TriggerDate { get; set; }
    public bool Applied { get; set; }

    public Boutique Boutique { get; set; }
}
