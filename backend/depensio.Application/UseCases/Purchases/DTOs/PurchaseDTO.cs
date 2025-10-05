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
}
