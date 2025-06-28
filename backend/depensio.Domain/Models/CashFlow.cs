namespace depensio.Domain.Models;

public class CashFlow : Entity<Guid>
{
    public Guid BoutiqueId { get; set; }
    public string Label { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime Date { get; set; }
    public string RecordedBy { get; set; }

    public Boutique Boutique { get; set; }
}
