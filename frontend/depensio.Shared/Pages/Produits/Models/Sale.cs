namespace depensio.Shared.Pages.Produits.Models;

public record GetSaleByBoutiqueResponse(IEnumerable<Sale> Sales);
public record CreateSaleRequest(Sale Sale);
public record CreateSaleResponse(Guid Id);
public record Sale
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid BoutiqueId { get; set; } = Guid.Empty;
    public decimal TotalPrice { get; set; } 
    public List<SaleItem> Items { get; set; } = new List<SaleItem>();    
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
