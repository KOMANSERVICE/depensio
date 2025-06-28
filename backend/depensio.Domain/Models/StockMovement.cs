namespace depensio.Domain.Models;

public class StockMovement : Entity<Guid>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string Type { get; set; }
    public Guid SourceLocationId { get; set; }
    public Guid DestinationLocationId { get; set; }
    public DateTime Date { get; set; }
    public string Reference { get; set; }

    public Product Product { get; set; }
    public StockLocation SourceLocation { get; set; }
    public StockLocation DestinationLocation { get; set; }
}
