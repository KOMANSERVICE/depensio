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
