using depensio.Shared.Models;
using depensio.Shared.Pages.Produits.Models;
using Refit;

namespace depensio.Shared.Services;

public interface IProductService
{
    [Post("/product")]
    Task<BaseResponse<CreateOrUpdateProductResponse>> CreateProductAsync(CreateProductRequest request);
    [Patch("/product")]
    Task<BaseResponse<CreateOrUpdateProductResponse>> UpdateProductByBoutique(CreateProductRequest request);
    [Get("/product/{boutiqueId}")]
    Task<BaseResponse<GetProductByUserResponse>> GetProductsByUserAsync(Guid boutiqueId);
    [Get("/product/{boutiqueId}/stock")]
    Task<BaseResponse<GetProductByUserResponse>> GetProductByBoutiqueWithStockSetting(Guid boutiqueId);

    
}
