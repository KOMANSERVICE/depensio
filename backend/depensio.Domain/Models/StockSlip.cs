namespace depensio.Domain.Models;
public class StockSlip : Entity<Guid>
{
    public string Reference { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    //public string Status { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public BoutiqueId BoutiqueId { get; set; }
    public Boutique Boutique { get; set; }
    //public ICollection<StockSlipItem> StockSlipItems { get; set; }
}