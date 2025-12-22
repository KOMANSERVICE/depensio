namespace depensio.Domain.Models;

public class PurchaseStatusHistory : Entity<PurchaseStatusHistoryId>
{
    public PurchaseId PurchaseId { get; set; }
    public int? FromStatus { get; set; }
    public int ToStatus { get; set; }
    public string? Comment { get; set; }

    public Purchase Purchase { get; set; }
}
