namespace depensio.Domain.Models; 

public class UsersBoutique
{
    public string UserId { get; set; }
    public Guid BoutiqueId { get; set; }
    //public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }

    //public ApplicationUser ApplicationUser { get; set; }
    public Boutique Boutique { get; set; }
}
