using System.ComponentModel.DataAnnotations;

namespace depensio.Domain.Models;

public class ProfileMenu : Entity<ProfileMenuId>
{
    public string UserId { get; set; }
    public MenuId MenuId { get; set; }
    public bool IsActive { get; set; }

    public virtual ApplicationUser? User { get; set; }
    public virtual Menu? Menu { get; set; }
}
