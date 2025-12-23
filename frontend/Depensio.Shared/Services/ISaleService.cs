using depensio.Shared.Models;
using depensio.Shared.Pages.Produits.Models;
using Refit;

namespace depensio.Shared.Services;

public interface ISaleService
{

    [Post("/sale")]
    Task<BaseResponse<CreateSaleResponse>> CreateSaleAsync(CreateSaleRequest request);
    [Get("/sale/{boutiqueId}")]
    Task<BaseResponse<GetSaleByBoutiqueResponse>> GetSaleByBoutiqueAsync(Guid boutiqueId);
    [Get("/sale/{boutiqueId}/summary")]
    Task<BaseResponse<GetSaleSummaryByBoutiqueResponse>> GetSaleSummaryByBoutiqueAsync(Guid boutiqueId);
    [Post("/sale/cancel")]
    Task<BaseResponse<CancelSaleResponse>> CancelSaleAsync(CancelSaleRequest request);

    [Get("/sale/{saleId}/history")]
    Task<BaseResponse<GetSaleHistoryResponse>> GetSaleHistoryAsync(Guid saleId, [Query] Guid boutiqueId);
}
