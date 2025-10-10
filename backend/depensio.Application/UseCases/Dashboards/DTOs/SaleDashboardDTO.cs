namespace depensio.Application.UseCases.Dashboard.DTOs;

public record SaleDashboardDTO
{
    public string Name { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal SalePrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPurchase { get; set; }
    public decimal AveragePurchasePrice { get; set; }
    public decimal Balance { get; set; }= 0;
    

}

public record SaleRequestDTO
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
