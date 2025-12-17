using depensio.Shared.Pages.Tresoreries.Models;

namespace depensio.Shared.Services;

public interface ITresorerieService
{
    [Get("/tresorerie/{boutiqueId}/accounts")]
    Task<BaseResponse<GetAccountsResponse>> GetAccountsAsync(
        Guid boutiqueId,
        bool includeInactive = false,
        AccountType? type = null);
}
