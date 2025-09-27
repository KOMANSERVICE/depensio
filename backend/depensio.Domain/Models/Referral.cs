namespace depensio.Domain.Models;

public class Referral : Entity<Guid>
{
    public string UserId { get; set; }
    public Guid ReferredUserId { get; set; }
    public bool BonusGiven { get; set; }
    public bool Used { get; set; }
    public DateTime Date { get; set; }

    public ApplicationUser ApplicationUser { get; set; }
}
