namespace depensio.Domain.Models;

public class Purchase : Entity<Guid>
{
    public Guid UserId { get; set; }
    public Guid BoutiqueId { get; set; }
    public string SupplierName { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }

    public Boutique Boutique { get; set; }
    //public ApplicationUser ApplicationUser { get; set; }
    public ICollection<PurchaseItem> PurchaseItems { get; set; }
}
