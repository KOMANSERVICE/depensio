using depensio.Domain.Enums;

namespace depensio.Domain.Models;

public class Sale : Entity<SaleId>
{
    public BoutiqueId BoutiqueId { get; set; }
    public DateTime Date { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Validated;
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    // Treasury integration fields (no SQL FK - references external Treasury microservice)
    public string? PaymentMethodId { get; set; }
    public Guid? AccountId { get; set; }
    public string? CategoryId { get; set; }
    public Guid? CashFlowId { get; set; }
    /// <summary>
    /// Reference to the reversal CashFlow created when the sale is cancelled (contre-passation)
    /// </summary>
    public Guid? ReversalCashFlowId { get; set; }
    public decimal TotalAmount { get; set; }

    public Boutique Boutique { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; }
    public ICollection<SaleStatusHistory> StatusHistory { get; set; }
}
