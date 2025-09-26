using depensio.Domain.ValueObjects;

namespace depensio.Domain.Models; 

public class UsersBoutique : Entity<UsersBoutiqueId>
{
    public Guid UserId { get; set; }
    public BoutiqueId BoutiqueId { get; set; }
    //public string Role { get; set; }

    public ApplicationUser User { get; set; }
    public Boutique Boutique { get; set; }
}
