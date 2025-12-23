namespace depensio.Shared.Pages.Produits.Models;

public record GetPurchaseByBoutiqueResponse(IEnumerable<Purchase> Purchases);
public record CreatePurchaseRequest(Purchase Purchase);
public record CreatePurchaseResponse(Guid Id);
public record UpdatePurchaseRequest(Purchase Purchase);
public record UpdatePurchaseResponse(Guid Id);
public record SubmitPurchaseRequest(Guid BoutiqueId);
public record SubmitPurchaseResponse(Guid Id, string Status);
public record ApprovePurchaseRequest(Guid BoutiqueId);
public record ApprovePurchaseResponse(Guid Id, string Status, Guid? CashFlowId);
public record RejectPurchaseRequest(Guid BoutiqueId, string Reason);
public record RejectPurchaseResponse(Guid Id, string Status);
public record ReopenPurchaseRequest(Guid BoutiqueId);
public record ReopenPurchaseResponse(Guid Id, string Status);
public record CancelPurchaseRequest(Guid BoutiqueId, string? Reason);
public record CancelPurchaseResponse(Guid Id, string Status);
public record TransferPurchaseRequest(
    Guid BoutiqueId,
    string? PaymentMethod = null,
    Guid? AccountId = null,
    string? CategoryId = null
);
public record TransferPurchaseResponse(Guid Id, string Status, Guid? CashFlowId);

/// <summary>
/// Response containing the status history of a purchase
/// </summary>
public record GetPurchaseHistoryResponse(IEnumerable<PurchaseStatusHistory> History);

/// <summary>
/// Represents a single status change entry in the purchase history
/// </summary>
public record PurchaseStatusHistory
{
    /// <summary>
    /// The unique identifier of the history entry
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The date and time when the status change occurred
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// The user who made the status change
    /// </summary>
    public string ChangedBy { get; init; } = string.Empty;

    /// <summary>
    /// The previous status (null if initial creation)
    /// </summary>
    public string? FromStatus { get; init; }

    /// <summary>
    /// The new status after the change
    /// </summary>
    public string ToStatus { get; init; } = string.Empty;

    /// <summary>
    /// Optional comment about the status change
    /// </summary>
    public string? Comment { get; init; }
}

public record Purchase
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid BoutiqueId { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public DateOnly DateAchat { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public string SupplierName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal TotalQuantity { get; set; }
    public List<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();

    /// <summary>
    /// Status of the purchase: "draft" for draft, null/empty for approved (default)
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Optional payment method (e.g., "CASH", "BANK_TRANSFER", "MOBILE_MONEY", etc.)
    /// Required when approving and creating CashFlow
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Optional account ID (required when approving and creating CashFlow)
    /// </summary>
    public Guid? AccountId { get; set; }

    /// <summary>
    /// Optional expense category ID (required when approving and creating CashFlow)
    /// </summary>
    public string? CategoryId { get; set; }

    /// <summary>
    /// Indicates whether the purchase has been successfully transferred to the treasury (CashFlow created)
    /// </summary>
    public bool IsTransferred { get; set; }

    /// <summary>
    /// The ID of the CashFlow created in Treasury when the purchase was approved (if transferred)
    /// </summary>
    public Guid? CashFlowId { get; set; }
}

public record PurchaseItem
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid ProductId { get; set; } = Guid.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}