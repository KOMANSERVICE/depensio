using depensio.Domain.ValueObjects;

namespace depensio.Domain.Models; 

public class UsersBoutique : Entity<UsersBoutiqueId>
{
    public string UserId { get; set; }
    public BoutiqueId BoutiqueId { get; set; }
    //public string Role { get; set; }

    public virtual ApplicationUser User { get; set; }
    public virtual Boutique Boutique { get; set; }
    public virtual Profile? Profile { get; set; }
}
