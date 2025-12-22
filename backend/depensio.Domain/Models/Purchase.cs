namespace depensio.Domain.Models;

public class Purchase : Entity<PurchaseId>
{
    public BoutiqueId BoutiqueId { get; set; }
    public string SupplierName { get; set; }
    public DateOnly DateAchat { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    // New fields for purchase workflow
    public int Status { get; set; }
    public string? PaymentMethod { get; set; }
    public Guid? AccountId { get; set; }
    public string? CategoryId { get; set; }
    public Guid? CashFlowId { get; set; }
    /// <summary>
    /// Indicates whether the purchase has been successfully transferred to the treasury (CashFlow created)
    /// </summary>
    public bool IsTransferred { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? RejectionReason { get; set; }

    public Boutique Boutique { get; set; }
    public ICollection<PurchaseItem> PurchaseItems { get; set; }
    public ICollection<PurchaseStatusHistory> StatusHistory { get; set; }
}
