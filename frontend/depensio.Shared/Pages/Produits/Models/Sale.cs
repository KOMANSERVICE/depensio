namespace depensio.Shared.Pages.Produits.Models;

public record GetSaleByBoutiqueResponse(IEnumerable<Sale> Sales);
public record CreateSaleRequest(Sale Sale);
public record CreateSaleResponse(Guid Id);
public record CancelSaleRequest(Guid SaleId, string? Reason);
public record CancelSaleResponse(bool Success);

public enum SaleStatus
{
    Validated = 1,
    Cancelled = 2
}

public record Sale
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid BoutiqueId { get; set; } = Guid.Empty;
    public DateTime Date { get; set; }
    public int Status { get; set; } = 1;
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public decimal TotalPrice { get; set; }
    public List<SaleItem> Items { get; set; } = new List<SaleItem>();

    public bool IsCancelled => Status == (int)SaleStatus.Cancelled;
    public string StatusLabel => Status == (int)SaleStatus.Cancelled ? "Annulée" : "Validée";
}

public record SaleItem
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid ProductId { get; set; } = Guid.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public List<string> Barcodes { get; set; } = new();
}

public record GetSaleSummaryByBoutiqueResponse(IEnumerable<SaleSummary> SaleSummarys);
public record SaleSummary(Guid ProductId, string ProductName, int TotalQuantity, decimal TotalRevenue, string Color);
