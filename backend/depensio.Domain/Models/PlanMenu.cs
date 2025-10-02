namespace depensio.Domain.Models;

public class PlanMenu : Entity<PlanMenuId>
{
    public PlanId PlanId { get; set; }
    public MenuId MenuId { get; set; }

    public virtual Plan Plan { get; set; } = new();
    public virtual Menu Menu { get; set; } = new();
}
