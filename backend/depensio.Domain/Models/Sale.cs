

namespace depensio.Domain.Models;

public class Sale : Entity<SaleId>
{
    public BoutiqueId BoutiqueId { get; set; }
    public DateTime Date { get; set; }
    //public string Title { get; set; } = string.Empty;
    //public string Description { get; set; } = string.Empty;
    //public decimal TotalAmount { get; set; }

    public Boutique Boutique { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; }
}
