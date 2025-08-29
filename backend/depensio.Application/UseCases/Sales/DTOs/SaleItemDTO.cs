namespace depensio.Application.UseCases.Sales.DTOs;
public record SaleItemDTO
(
    Guid Id,
    Guid ProductId,
    decimal Price,
    int Quantity,
    List<string> Barcodes
);