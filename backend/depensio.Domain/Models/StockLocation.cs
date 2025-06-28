namespace depensio.Domain.Models;

public class StockLocation : Entity<Guid>
{
    public string Name { get; set; }
    public string Type { get; set; }
    public Guid BoutiqueId { get; set; }

    public Boutique Boutique { get; set; }
    public ICollection<StockMovement> SourceMovements { get; set; }
    public ICollection<StockMovement> DestinationMovements { get; set; }
}
