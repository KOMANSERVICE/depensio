using depensio.Shared.Models;
using depensio.Shared.Pages.Produits.Models;
using Refit;

namespace depensio.Shared.Services;

public interface IPurchaseService
{

    [Post("/purchase")]
    Task<BaseResponse<CreatePurchaseResponse>> CreatePurchaseAsync(CreatePurchaseRequest request);

    [Put("/purchase/{id}")]
    Task<BaseResponse<UpdatePurchaseResponse>> UpdatePurchaseAsync(Guid id, UpdatePurchaseRequest request);

    [Get("/purchase/{boutiqueId}")]
    Task<BaseResponse<GetPurchaseByBoutiqueResponse>> GetPurchaseByBoutiqueAsync(Guid boutiqueId);

    [Post("/purchase/{id}/submit")]
    Task<BaseResponse<SubmitPurchaseResponse>> SubmitPurchaseAsync(Guid id, SubmitPurchaseRequest request);

    [Post("/purchase/{id}/approve")]
    Task<BaseResponse<ApprovePurchaseResponse>> ApprovePurchaseAsync(Guid id, ApprovePurchaseRequest request);

    [Post("/purchase/{id}/reject")]
    Task<BaseResponse<RejectPurchaseResponse>> RejectPurchaseAsync(Guid id, RejectPurchaseRequest request);

    [Post("/purchase/{id}/reopen")]
    Task<BaseResponse<ReopenPurchaseResponse>> ReopenPurchaseAsync(Guid id, ReopenPurchaseRequest request);

    [Post("/purchase/{id}/cancel")]
    Task<BaseResponse<CancelPurchaseResponse>> CancelPurchaseAsync(Guid id, CancelPurchaseRequest request);

    [Get("/purchase/{id}/history")]
    Task<BaseResponse<GetPurchaseHistoryResponse>> GetPurchaseHistoryAsync(Guid id, [Query] Guid boutiqueId);
}
