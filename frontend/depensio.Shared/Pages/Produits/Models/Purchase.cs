namespace depensio.Shared.Pages.Produits.Models;

public record GetPurchaseByBoutiqueResponse(IEnumerable<Purchase> Purchases);
public record CreatePurchaseRequest(Purchase Purchase);
public record CreatePurchaseResponse(Guid Id);
public record Purchase
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid BoutiqueId { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal TotalQuantity { get; set; }
    public List<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}

public record PurchaseItem
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid ProductId { get; set; } = Guid.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}