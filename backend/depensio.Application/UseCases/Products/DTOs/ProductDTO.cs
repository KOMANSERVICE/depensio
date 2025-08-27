using depensio.Domain.Enums;

namespace depensio.Application.UseCases.Products.DTOs;

public record ProductDTO(
    Guid Id, 
    Guid BoutiqueId, 
    string Name, 
    string Barcode, decimal Price, 
    decimal CostPrice, int Stock);

public record ProductUpdateDTO(
    Guid Id,
    Guid BoutiqueId,
    string Name, decimal Price,
    decimal CostPrice, int Stock);
public record ProductItemDTO(
    Guid Id, Guid BoutiqueId,
    Guid ProductId, int BarcodeCount, 
    decimal DiscountValue, DiscountType DiscountType);

public record BarcodeDTO(Guid BoutiqueId,List<string> Barcodes);


