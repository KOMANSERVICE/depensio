using depensio.Domain.Enums;

namespace depensio.Application.UseCases.Purchases.DTOs;

public record PurchaseDTO
{
    public Guid Id { get; set; }
    public Guid BoutiqueId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateOnly DateAchat { get; set; }
    public string Description { get; set; } = string.Empty;
    public IEnumerable<PurchaseItemDTO> Items { get; set; } = new List<PurchaseItemDTO>();
    public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
    public decimal TotalQuantity => Items.Sum(item => item.Quantity);

    /// <summary>
    /// Status of the purchase. Use "draft" to create a draft purchase.
    /// If not specified, defaults to Approved for backward compatibility.
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
}
