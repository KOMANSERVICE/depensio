namespace depensio.Application.UseCases.Sales.DTOs;

public record SaleDTO
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid BoutiqueId { get; set; } = Guid.Empty;
    public DateTime Date { get; set; }
    public int Status { get; set; } = 1;
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);

    public IEnumerable<SaleItemDTO> Items { get; set; } = new List<SaleItemDTO>();
}