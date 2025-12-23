namespace depensio.Domain.Models;

public class SaleStatusHistory : Entity<SaleStatusHistoryId>
{
    public SaleId SaleId { get; set; }
    public int? FromStatus { get; set; }
    public int ToStatus { get; set; }
    public string? Comment { get; set; }

    public Sale Sale { get; set; }
}
