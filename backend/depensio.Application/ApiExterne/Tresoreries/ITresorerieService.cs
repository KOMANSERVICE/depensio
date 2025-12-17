using IDR.Library.Shared.Responses;
using Refit;

namespace depensio.Application.ApiExterne.Tresoreries;

public interface ITresorerieService
{
    [Post("/api/accounts")]
    Task<BaseResponse<CreateAccountResponse>> CreateAccountAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] CreateAccountRequest request);

    [Get("/api/accounts")]
    Task<BaseResponse<GetAccountsResponse>> GetAccountsAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Query] bool includeInactive = false,
        [Query] AccountType? type = null);
}

public enum AccountType
{
    Cash = 0,
    Bank = 1,
    MobileMoney = 2,
    Other = 3
}

public record CreateAccountRequest(
    string Name,
    string? Description,
    AccountType Type,
    decimal InitialBalance,
    decimal? AlertThreshold,
    decimal? OverdraftLimit,
    string? AccountNumber,
    string? BankName,
    bool IsDefault
);

public record CreateAccountResponse(AccountDTO Account);

public record AccountDTO(
    Guid Id,
    string ApplicationId,
    string BoutiqueId,
    string Name,
    string? Description,
    AccountType Type,
    string? AccountNumber,
    string? BankName,
    decimal InitialBalance,
    decimal CurrentBalance,
    string Currency,
    bool IsActive,
    bool IsDefault,
    decimal? OverdraftLimit,
    decimal? AlertThreshold,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record GetAccountsResponse(
    IReadOnlyList<AccountListDto> Accounts,
    decimal TotalAvailable,
    int TotalCount
);

public record AccountListDto(
    Guid Id,
    string Name,
    AccountType Type,
    decimal CurrentBalance,
    string Currency,
    bool IsActive,
    bool IsDefault,
    decimal? AlertThreshold,
    bool IsInAlert
);
