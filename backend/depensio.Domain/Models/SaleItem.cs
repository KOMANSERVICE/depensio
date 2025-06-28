namespace depensio.Domain.Models;

public class SaleItem : Entity<Guid>
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public Sale Sale { get; set; }
    public Product Product { get; set; }
}
