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

    [Get("/tresorerie/{boutiqueId}/accounts/{accountId}/balance")]
    Task<BaseResponse<AccountBalanceDto>> GetAccountBalanceAsync(
        Guid boutiqueId,
        Guid accountId);

    [Post("/tresorerie/{boutiqueId}/accounts")]
    Task<BaseResponse<CreateAccountResponse>> CreateAccountAsync(
        Guid boutiqueId,
        CreateAccountRequest request);

    [Post("/tresorerie/categories")]
    Task<BaseResponse<CreateCategoryResponse>> CreateCategoryAsync(
        [Body] CreateCategoryRequest request);

    [Get("/tresorerie/categories")]
    Task<BaseResponse<GetCategoriesResponse>> GetCategoriesAsync(
        CategoryType? type = null,
        bool includeInactive = false);
}
