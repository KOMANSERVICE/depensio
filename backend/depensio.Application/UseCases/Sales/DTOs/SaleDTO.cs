namespace depensio.Application.UseCases.Sales.DTOs;

public record SaleDTO
(
    Guid Id, 
    Guid BoutiqueId,     
    string Title,
    string Description,
    decimal TotalPrice,
    IEnumerable<SaleItemDTO> Items);
