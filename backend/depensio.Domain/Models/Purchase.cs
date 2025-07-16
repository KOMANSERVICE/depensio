namespace depensio.Domain.Models;

public class Purchase : Entity<PurchaseId>
{
    public BoutiqueId BoutiqueId { get; set; }
    public string SupplierName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    //public decimal TotalAmount { get; set; }

    public Boutique Boutique { get; set; }
    public ICollection<PurchaseItem> PurchaseItems { get; set; }
}
