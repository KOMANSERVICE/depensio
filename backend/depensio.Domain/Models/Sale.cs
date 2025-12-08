

using depensio.Domain.Enums;

namespace depensio.Domain.Models;

public class Sale : Entity<SaleId>
{
    public BoutiqueId BoutiqueId { get; set; }
    public DateTime Date { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Validated;
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    public Boutique Boutique { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; }
}
