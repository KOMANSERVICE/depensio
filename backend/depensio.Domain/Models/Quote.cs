namespace depensio.Domain.Models;

public class Quote : Entity<Guid>
{
    public Guid BoutiqueId { get; set; }
    public string ClientName { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }

    public Boutique Boutique { get; set; }
}
