

namespace depensio.Shared.Pages.Tresoreries.Models;


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
    INCOME = 1,
    EXPENSE = 2,
    TRANSFER = 3
}

public enum CashFlowStatus
{
    DRAFT = 1,
    PENDING = 2,
    APPROVED = 3,
    REJECTED = 4,
    CANCELLED = 5
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
    CashFlowStatusExtended Status,
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

public record CreateAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AccountType Type { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal? AlertThreshold { get; set; }
    public decimal? OverdraftLimit { get; set; }
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public bool IsDefault { get; set; }
}

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
    public AccountType Type { get; set; } = AccountType.CASH;
    public decimal InitialBalance { get; set; }
    public decimal? AlertThreshold { get; set; }
    public decimal? OverdraftLimit { get; set; }
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public bool IsDefault { get; set; }
}

// CashFlow DTOs
public enum CashFlowTypeExtended
{
    INCOME = 1,
    EXPENSE = 2,
    TRANSFER = 3
}

public enum CashFlowStatusExtended
{
    DRAFT = 1,
    PENDING = 2,
    APPROVED = 3,
    REJECTED = 4,
    CANCELLED = 5
}

public enum ThirdPartyType
{
    SUPPLIER = 1,
    CUSTOMER = 2,
    EMPLOYEE = 3,
    OTHER = 4
}

public record CreateCashFlowRequest(
    CashFlowTypeExtended Type,
    string CategoryId,
    string Label,
    string? Description,
    decimal Amount,
    Guid AccountId,
    string PaymentMethod,
    DateTime Date,
    string? CustomerName,
    string? SupplierName,
    string? AttachmentUrl
);

public record CreateCashFlowResponse(
    CashFlowDTO CashFlow,
    string? BudgetWarning
);

public record CashFlowDTO(
    Guid Id,
    string ApplicationId,
    string BoutiqueId,
    string? Reference,
    CashFlowTypeExtended Type,
    CashFlowStatusExtended Status,
    string CategoryId,
    string CategoryName,
    string Label,
    string? Description,
    decimal Amount,
    decimal TaxAmount,
    decimal TaxRate,
    string Currency,
    Guid AccountId,
    string AccountName,
    Guid? DestinationAccountId,
    string? DestinationAccountName,
    string PaymentMethod,
    DateTime Date,
    ThirdPartyType? ThirdPartyType,
    string? ThirdPartyName,
    string? ThirdPartyId,
    string? AttachmentUrl,
    DateTime? SubmittedAt,
    string? SubmittedBy,
    DateTime? ValidatedAt,
    string? ValidatedBy,
    string? RejectionReason,
    bool IsReconciled = false,
    DateTime? ReconciledAt = null,
    string? ReconciledBy = null,
    string? BankStatementReference = null
);

public class CashFlowCreateDTO
{
    public CashFlowTypeExtended Type { get; set; } = CashFlowTypeExtended.EXPENSE;
    public string CategoryId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
    public string PaymentMethod { get; set; } = "CASH";
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? CustomerName { get; set; }
    public string? SupplierName { get; set; }
    public string? AttachmentUrl { get; set; }
}

// Transfer DTOs
public record CreateTransferRequest(
    Guid AccountId,
    Guid DestinationAccountId,
    decimal Amount,
    DateTime Date,
    string Label,
    string? Description
);

public record CreateTransferResponse(
    TransferDto Transfer
);

public record TransferDto(
    Guid Id,
    string Type,
    string Status,
    Guid AccountId,
    string AccountName,
    Guid DestinationAccountId,
    string DestinationAccountName,
    decimal Amount,
    DateTime Date,
    string Label,
    string? Description,
    decimal SourceAccountBalance,
    decimal DestinationAccountBalance,
    DateTime CreatedAt,
    string CreatedBy
);

public class TransferCreateDTO
{
    public Guid AccountId { get; set; }
    public Guid DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
}

// UpdateCashFlow DTOs
public record UpdateCashFlowRequest(
    string? CategoryId,
    string? Label,
    string? Description,
    decimal? Amount,
    decimal? TaxAmount,
    decimal? TaxRate,
    string? Currency,
    Guid? AccountId,
    Guid? DestinationAccountId,
    string? PaymentMethod,
    DateTime? Date,
    ThirdPartyType? ThirdPartyType,
    string? ThirdPartyName,
    string? ThirdPartyId,
    string? AttachmentUrl
);

public record UpdateCashFlowResponse(CashFlowDTO CashFlow);

public record GetCashFlowResponse(CashFlowDTO CashFlow);

public record SubmitCashFlowResponse(
    CashFlowDTO CashFlow,
    string? BudgetWarning
);

public record ApproveCashFlowResponse(
    CashFlowDTO CashFlow,
    decimal NewAccountBalance
);

public record RejectCashFlowRequest(string RejectionReason);

public record RejectCashFlowResponse(CashFlowDTO CashFlow);

public class CashFlowEditDTO
{
    public Guid Id { get; set; }
    public CashFlowTypeExtended Type { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
    public string PaymentMethod { get; set; } = "CASH";
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? CustomerName { get; set; }
    public string? SupplierName { get; set; }
    public string? AttachmentUrl { get; set; }
}

// GetCashFlows DTOs
public record GetCashFlowsResponse(
    IReadOnlyList<CashFlowListDto> CashFlows,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasPrevious,
    bool HasNext
);

public record CashFlowListDto(
    Guid Id,
    string? Reference,
    CashFlowTypeExtended Type,
    CashFlowStatusExtended Status,
    string CategoryId,
    string CategoryName,
    string Label,
    decimal Amount,
    string Currency,
    Guid AccountId,
    string AccountName,
    Guid? DestinationAccountId,
    string? DestinationAccountName,
    string PaymentMethod,
    DateTime Date,
    string? ThirdPartyName
);

// GetPendingCashFlows DTOs
public record GetPendingCashFlowsResponse(
    IReadOnlyList<PendingCashFlowDto> CashFlows,
    int PendingCount
);

public record PendingCashFlowDto(
    Guid Id,
    string? Reference,
    CashFlowTypeExtended Type,
    string CategoryId,
    string CategoryName,
    string Label,
    decimal Amount,
    string Currency,
    Guid AccountId,
    string AccountName,
    Guid? DestinationAccountId,
    string? DestinationAccountName,
    string PaymentMethod,
    DateTime Date,
    string? ThirdPartyName,
    DateTime? SubmittedAt,
    string? SubmittedBy
);

public record CreateRecurringCashFlowRequest(
    CashFlowTypeExtended Type,
    string CategoryId,
    string Label,
    string? Description,
    decimal Amount,
    Guid AccountId,
    string PaymentMethod,
    string? ThirdPartyName,
    RecurringFrequency Frequency,
    int Interval,
    int? DayOfMonth,
    int? DayOfWeek,
    DateTime StartDate,
    DateTime? EndDate,
    bool AutoValidate
);

public record CreateRecurringCashFlowResponse(
    RecurringCashFlowDTO RecurringCashFlow
);

public record RecurringCashFlowDTO(
    Guid Id,
    string ApplicationId,
    string BoutiqueId,
    CashFlowTypeExtended Type,
    string CategoryId,
    string CategoryName,
    string Label,
    string? Description,
    decimal Amount,
    Guid AccountId,
    string AccountName,
    string PaymentMethod,
    string? ThirdPartyName,
    RecurringFrequency Frequency,
    int Interval,
    int? DayOfMonth,
    int? DayOfWeek,
    DateTime StartDate,
    DateTime? EndDate,
    DateTime NextOccurrence,
    bool AutoValidate,
    bool IsActive,
    DateTime? LastGeneratedAt
);

public class RecurringCashFlowCreateDTO
{
    public CashFlowTypeExtended Type { get; set; } = CashFlowTypeExtended.EXPENSE;
    public string CategoryId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
    public string PaymentMethod { get; set; } = "CASH";
    public string? ThirdPartyName { get; set; }
    public RecurringFrequency Frequency { get; set; } = RecurringFrequency.MONTHLY;
    public int Interval { get; set; } = 1;
    public int? DayOfMonth { get; set; }
    public int? DayOfWeek { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public bool AutoValidate { get; set; }
}

// GetRecurringCashFlows DTOs
public record GetRecurringCashFlowsResponse(
    IReadOnlyList<RecurringCashFlowDTO> RecurringCashFlows,
    int TotalCount,
    decimal EstimatedMonthlyTotal
);

// UpdateRecurringCashFlow DTOs
public record UpdateRecurringCashFlowRequest(
    CashFlowTypeExtended? Type,
    string? CategoryId,
    string? Label,
    string? Description,
    decimal? Amount,
    Guid? AccountId,
    string? PaymentMethod,
    string? ThirdPartyName,
    RecurringFrequency? Frequency,
    int? Interval,
    int? DayOfMonth,
    int? DayOfWeek,
    DateTime? StartDate,
    DateTime? EndDate,
    bool? AutoValidate,
    bool? IsActive
);

public record UpdateRecurringCashFlowResponse(
    RecurringCashFlowDTO RecurringCashFlow
);

// ToggleRecurringCashFlow DTOs
public record ToggleRecurringCashFlowResponse(
    Guid Id,
    bool IsActive
);

public class RecurringCashFlowEditDTO
{
    public Guid Id { get; set; }
    public CashFlowTypeExtended Type { get; set; } = CashFlowTypeExtended.EXPENSE;
    public string CategoryId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
    public string PaymentMethod { get; set; } = "CASH";
    public string? ThirdPartyName { get; set; }
    public RecurringFrequency Frequency { get; set; } = RecurringFrequency.MONTHLY;
    public int Interval { get; set; } = 1;
    public int? DayOfMonth { get; set; }
    public int? DayOfWeek { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public bool AutoValidate { get; set; }
    public bool IsActive { get; set; } = true;
}

// ReconcileCashFlow DTOs
public record ReconcileCashFlowRequest(
    string? BankStatementReference
);

public record ReconcileCashFlowResponse(
    CashFlowDetailDto CashFlow
);

public record CashFlowDetailDto(
    Guid Id,
    string ApplicationId,
    string BoutiqueId,
    string? Reference,
    CashFlowTypeExtended Type,
    CashFlowStatusExtended Status,
    string CategoryId,
    string CategoryName,
    string Label,
    string? Description,
    decimal Amount,
    decimal TaxAmount,
    decimal TaxRate,
    string Currency,
    Guid AccountId,
    string AccountName,
    Guid? DestinationAccountId,
    string? DestinationAccountName,
    string PaymentMethod,
    DateTime Date,
    ThirdPartyType? ThirdPartyType,
    string? ThirdPartyName,
    string? ThirdPartyId,
    string? AttachmentUrl,
    string? RelatedType,
    string? RelatedId,
    bool IsRecurring,
    string? RecurringCashFlowId,
    bool IsSystemGenerated,
    bool AutoApproved,
    bool IsReconciled,
    DateTime? ReconciledAt,
    string? ReconciledBy,
    string? BankStatementReference,
    DateTime? SubmittedAt,
    string? SubmittedBy,
    DateTime? ValidatedAt,
    string? ValidatedBy,
    string? RejectionReason,
    string? BudgetId,
    string? BudgetName,
    decimal? BudgetImpact
);

// GetUnreconciledCashFlows DTOs
public record GetUnreconciledCashFlowsResponse(
    IReadOnlyList<UnreconciledCashFlowDto> CashFlows,
    int UnreconciledCount,
    decimal TotalUnreconciledAmount
);

public record UnreconciledCashFlowDto(
    Guid Id,
    string? Reference,
    CashFlowTypeExtended Type,
    string CategoryId,
    string CategoryName,
    string Label,
    decimal Amount,
    string Currency,
    Guid AccountId,
    string AccountName,
    Guid? DestinationAccountId,
    string? DestinationAccountName,
    string PaymentMethod,
    DateTime Date,
    ThirdPartyType? ThirdPartyType,
    string? ThirdPartyName,
    DateTime? ValidatedAt,
    string? ValidatedBy
);

// Budget DTOs
public enum BudgetType
{
    GLOBAL = 1,
    CATEGORY = 2,
    PROJECT = 3
}

public record GetBudgetByIdResponse(BudgetDetailDto Budget);

public record BudgetDetailDto(
    Guid Id,
    string ApplicationId,
    string BoutiqueId,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    decimal AllocatedAmount,
    decimal SpentAmount,
    decimal RemainingAmount,
    decimal PercentUsed,
    string Currency,
    BudgetType Type,
    int AlertThreshold,
    bool IsExceeded,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<BudgetExpenseDto> Expenses,
    IReadOnlyList<CategoryBreakdownDto> CategoryBreakdown,
    IReadOnlyList<TimeSeriesDto> TimeEvolution
);

public record BudgetExpenseDto(
    Guid Id,
    string Label,
    string? Description,
    decimal Amount,
    string Currency,
    DateTime Date,
    string CategoryId,
    string CategoryName,
    CashFlowStatus Status,
    string? ThirdPartyName
);

public record CategoryBreakdownDto(
    Guid CategoryId,
    string CategoryName,
    string? CategoryIcon,
    decimal Amount,
    decimal Percentage,
    int TransactionCount
);

public record TimeSeriesDto(
    int Year,
    int Month,
    string MonthLabel,
    decimal Amount,
    decimal CumulativeAmount
);
