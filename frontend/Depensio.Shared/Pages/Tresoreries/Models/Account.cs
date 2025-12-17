namespace depensio.Shared.Pages.Tresoreries.Models;

public enum AccountType
{
    Cash = 0,
    Bank = 1,
    MobileMoney = 2,
    Other = 3
}

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

public class AccountEditDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? AlertThreshold { get; set; }
    public decimal? OverdraftLimit { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal? InitialBalance { get; set; }
}

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

public class AccountCreateDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AccountType Type { get; set; } = AccountType.Cash;
    public decimal InitialBalance { get; set; }
    public decimal? AlertThreshold { get; set; }
    public decimal? OverdraftLimit { get; set; }
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public bool IsDefault { get; set; }
}
