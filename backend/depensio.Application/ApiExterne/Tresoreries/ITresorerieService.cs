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

    [Put("/api/accounts/{accountId}")]
    Task<BaseResponse<UpdateAccountResponse>> UpdateAccountAsync(
        Guid accountId,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] UpdateAccountRequest request);

    [Get("/api/accounts/{id}")]
    Task<BaseResponse<GetAccountDetailResponse>> GetAccountDetailAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Query] DateTime? fromDate = null,
        [Query] DateTime? toDate = null);

    [Get("/api/accounts/{id}/balance")]
    Task<BaseResponse<AccountBalanceDto>> GetAccountBalanceAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId);
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

public record UpdateAccountRequest(
    string Name,
    decimal? AlertThreshold,
    decimal? OverdraftLimit,
    bool IsActive,
    decimal? InitialBalance = null
);

public record UpdateAccountResponse(Guid Id);

public enum CashFlowType
{
    Income = 0,
    Expense = 1
}

public enum CashFlowStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}

public record GetAccountDetailResponse(
    Guid Id,
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
    bool IsInAlert,
    decimal TotalIncome,
    decimal TotalExpense,
    IReadOnlyList<CashFlowMovementDto> RecentMovements,
    IReadOnlyList<BalanceEvolutionDto>? BalanceEvolution,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CashFlowMovementDto(
    Guid Id,
    string? Reference,
    CashFlowType Type,
    string Label,
    string? Description,
    decimal Amount,
    string Currency,
    DateTime Date,
    CashFlowStatus Status,
    string? ThirdPartyName
);

public record BalanceEvolutionDto(
    DateTime Date,
    decimal Balance,
    decimal TotalIncome,
    decimal TotalExpense
);

public record AccountBalanceDto(
    Guid AccountId,
    string AccountName,
    decimal CurrentBalance,
    string Currency,
    decimal VariationToday,
    decimal VariationThisMonth,
    bool IsAlertTriggered,
    decimal? AlertThreshold,
    DateTime CalculatedAt
);
