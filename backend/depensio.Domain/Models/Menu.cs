using depensio.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace depensio.Domain.Models;

public class Menu : Entity<MenuId>
{
    public string Name { get; set; } = string.Empty;

    public string ApiRoute { get; set; } = string.Empty;
    public string UrlFront { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public virtual ICollection<PlanMenu> PlanMenus { get; set; } = new HashSet<PlanMenu>();
    public virtual ICollection<ProfileMenu> ProfileMenus { get; set; } = new HashSet<ProfileMenu>();
}
