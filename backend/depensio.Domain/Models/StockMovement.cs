using depensio.Domain.Enums;

namespace depensio.Domain.Models;

public class StockMovement : Entity<StockMovementId>
{
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    public string Reference { get; set; } = string.Empty;
    public StockMovementType MovementType { get; set; }

    public ProductId ProductId { get; set; }
    public StockLocationId SourceLocationId { get; set; }
    public StockLocationId DestinationLocationId { get; set; }
    public Product Product { get; set; }
    public StockLocation SourceLocation { get; set; } 
    public StockLocation DestinationLocation { get; set; }
}
