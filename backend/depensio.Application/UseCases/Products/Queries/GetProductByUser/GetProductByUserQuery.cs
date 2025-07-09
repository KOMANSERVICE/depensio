using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Queries.GetProductByUser;

public record GetProductByUserQuery(Guid BoutiqueId)
    : IQuery<GetProductByUserResult>;
public record GetProductByUserResult(IEnumerable<ProductDTO> Products);