namespace depensio.Domain.Models;

public class Profile : Entity<ProfileId>
{
    public BoutiqueId BoutiqueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Relations
    public virtual Boutique? Boutique { get; set; }
    public virtual ICollection<ProfileMenu> ProfileMenus { get; set; } = new HashSet<ProfileMenu>();
    public virtual ICollection<UsersBoutique> UsersBoutiques { get; set; } = new HashSet<UsersBoutique>();
}
