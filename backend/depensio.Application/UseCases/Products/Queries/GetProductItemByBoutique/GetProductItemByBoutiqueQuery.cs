using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Queries.GetProductItemByBoutique;

public record GetProductItemByBoutiqueQuery(Guid BoutiqueId)
    : IQuery<GetProductItemByBoutiqueResult>;
public record GetProductItemByBoutiqueResult(IEnumerable<ProductBarcodeDTO> ProductBarcodes);