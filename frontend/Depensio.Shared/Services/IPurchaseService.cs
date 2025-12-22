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
}
