using depensio.Domain.ValueObjects;

namespace depensio.Domain.Models;

public class Product : Entity<ProductId>
{
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int Stock { get; set; }
   // public Guid CategoryId { get; set; }
    public BoutiqueId BoutiqueId { get; set; }

    public Boutique Boutique { get; set; }
    //public ICollection<StockMovement> StockMovements { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; }
    public ICollection<PurchaseItem> PurchaseItems { get; set; }
}
