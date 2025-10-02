using System.ComponentModel.DataAnnotations;

namespace depensio.Domain.Models;

public class ProfileMenu : Entity<ProfileMenuId>
{
    public ProfileId ProfileId { get; set; }
    public MenuId MenuId { get; set; }
    public bool IsActive { get; set; }

    public virtual Profile? Profiles { get; set; }
    public virtual Menu? Menu { get; set; }
}
