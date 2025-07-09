using depensio.Shared.Models;
using depensio.Shared.Pages.Produits.Models;
using Refit;

namespace depensio.Shared.Services;

public interface IProductService
{
    [Post("/product")]
    Task<BaseResponse<CreateProductResponse>> CreateProductAsync(CreateProductRequest request);
    [Get("/product/{boutiqueId}")]
    Task<BaseResponse<GetProductByUserResponse>> GetProductsByUserAsync(Guid boutiqueId);
}
