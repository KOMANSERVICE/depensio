using depensio.Domain.ValueObjects;

namespace depensio.Domain.Models;

public class SaleItem : Entity<SaleItemId>
{
    public SaleId SaleId { get; set; }
    public ProductId ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public Sale Sale { get; set; }
    public Product Product { get; set; }
}
