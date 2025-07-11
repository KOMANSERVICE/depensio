namespace depensio.Application.UseCases.Sales.DTOs;

public record SaleDTO
(
    Guid Id, 
    Guid BoutiqueId,     
    string Title,
    string Description,
    IEnumerable<SaleItemDTO> Items);
