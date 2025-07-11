using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Queries.GetProductByBoutique;

public record GetProductByBoutiqueQuery(Guid BoutiqueId)
    : IQuery<GetProductByBoutiqueResult>;
public record GetProductByBoutiqueResult(IEnumerable<ProductDTO> Products);