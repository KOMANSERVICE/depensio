namespace depensio.Application.UseCases.Products.DTOs;

public record ProductDTO(
    Guid Id, 
    Guid BoutiqueId, 
    string Name, 
    string Barcode, decimal Price, 
    decimal CostPrice, int Stock);

