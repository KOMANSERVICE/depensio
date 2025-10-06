namespace depensio.Shared.Pages.Dashboards.Models;

public record SaleDashboard
{
    public string Name { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal SalePrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AveragePurchasePrice { get; set; }
    public decimal Balance { get; set; } = 0;

}

public record SaleRequest
{
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}

public record GetSalesDetailByBoutiqueRequest(SaleRequest SaleRequest);
public record GetSalesDetailByBoutiqueResponse(IEnumerable<SaleDashboard> SalesDetails);
