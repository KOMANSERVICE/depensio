namespace depensio.Domain.Models;

public class PurchaseItem : Entity<PurchaseItemId>
{
    public PurchaseId PurchaseId { get; set; }
    public ProductId ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public Purchase Purchase { get; set; }
    public Product Product { get; set; }
}
