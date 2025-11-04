using depensio.Domain.Enums;

namespace depensio.Domain.Models;

public class StockLocation : Entity<StockLocationId>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; }
    public StockLocationType Type { get; set; }
    public BoutiqueId BoutiqueId { get; set; }
    public Boutique Boutique { get; set; }
    //public ICollection<StockMovement> SourceMovements { get; set; }
    //public ICollection<StockMovement> DestinationMovements { get; set; }
}
