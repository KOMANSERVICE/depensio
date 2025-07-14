using depensio.Shared.Models;
using depensio.Shared.Pages.Produits.Models;
using Refit;

namespace depensio.Shared.Services;

public interface ISaleService
{

    [Post("/sale")]
    Task<BaseResponse<CreateProductResponse>> CreateSaleAsync(CreateProductRequest request);
    [Get("/sale/{boutiqueId}")]
    Task<BaseResponse<GetSaleByBoutiqueResponse>> GetSaleByBoutiqueAsync(Guid boutiqueId);
}
