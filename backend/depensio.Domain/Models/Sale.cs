namespace depensio.Domain.Models;

public class Sale : Entity<Guid>
{
    public Guid BoutiqueId { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }

    public Boutique Boutique { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; }
}
