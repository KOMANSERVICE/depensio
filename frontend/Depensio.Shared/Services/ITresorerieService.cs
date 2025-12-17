using depensio.Shared.Pages.Tresoreries.Models;

namespace depensio.Shared.Services;

public interface ITresorerieService
{
    [Get("/tresorerie/{boutiqueId}/accounts")]
    Task<BaseResponse<GetAccountsResponse>> GetAccountsAsync(
        Guid boutiqueId,
        bool includeInactive = false,
        AccountType? type = null);

    [Put("/tresorerie/{boutiqueId}/accounts/{accountId}")]
    Task<BaseResponse<UpdateAccountResponse>> UpdateAccountAsync(
        Guid boutiqueId,
        Guid accountId,
        [Body] UpdateAccountRequest request);

    [Get("/tresorerie/{boutiqueId}/accounts/{accountId}")]
    Task<BaseResponse<GetAccountDetailResponse>> GetAccountDetailAsync(
        Guid boutiqueId,
        Guid accountId,
        DateTime? fromDate = null,
        DateTime? toDate = null);
}
