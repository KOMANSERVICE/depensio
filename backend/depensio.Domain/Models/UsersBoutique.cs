namespace depensio.Domain.Models; 

public class UsersBoutique : Entity<Guid>
{
    public string UserId { get; set; }
    public Guid BoutiqueId { get; set; }
    //public string Role { get; set; }

    //public ApplicationUser ApplicationUser { get; set; }
    public Boutique Boutique { get; set; }
}
