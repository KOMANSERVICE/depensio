namespace depensio.Application.UseCases.Sales.DTOs;

public record SaleDTO
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid BoutiqueId { get; set; } = Guid.Empty;
    //public string Title { get; set; } = string.Empty;
    //public string Description { get; set; } = string.Empty;

    public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);

    public IEnumerable<SaleItemDTO> Items { get; set; } = new List<SaleItemDTO>();
}