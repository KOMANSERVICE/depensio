namespace depensio.Domain.Models;

public class Plan : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int ProductLimit { get; set; }
    public int UserLimit { get; set; }
    public string Features { get; set; } = string.Empty;

    public ICollection<Subscription> Subscriptions { get; set; }
}
