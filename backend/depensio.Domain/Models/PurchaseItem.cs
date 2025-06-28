namespace depensio.Domain.Models;

public class PurchaseItem : Entity<Guid>
{
    public Guid PurchaseId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public Purchase Purchase { get; set; }
    public Product Product { get; set; }
}
