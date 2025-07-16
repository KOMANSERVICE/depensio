namespace depensio.Application.UseCases.Sales.DTOs;

public record SaleSummaryDTO(Guid ProductId,string ProductName, int TotalQuantity, decimal TotalRevenue);
