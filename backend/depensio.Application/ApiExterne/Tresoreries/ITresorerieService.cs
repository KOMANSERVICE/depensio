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

    [Post("/api/categories")]
    Task<BaseResponse<CreateCategoryResponse>> CreateCategoryAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] CreateCategoryRequest request);

    [Get("/api/categories")]
    Task<BaseResponse<GetCategoriesResponse>> GetCategoriesAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Query] CategoryType? type = null,
        [Query] bool includeInactive = false);

    [Post("/api/cash-flows")]
    Task<BaseResponse<CreateCashFlowResponse>> CreateCashFlowAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] CreateCashFlowRequest request);

    [Post("/api/cash-flows/transfer")]
    Task<BaseResponse<CreateTransferResponse>> CreateTransferAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] CreateTransferRequest request);

    [Put("/api/cash-flows/{id}")]
    Task<BaseResponse<UpdateCashFlowResponse>> UpdateCashFlowAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] UpdateCashFlowRequest request);

    [Get("/api/cash-flows/{id}")]
    Task<BaseResponse<GetCashFlowResponse>> GetCashFlowAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId);

    [Delete("/api/cash-flows/{id}")]
    Task<IApiResponse> DeleteCashFlowAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId);

    [Post("/api/cash-flows/{id}/submit")]
    Task<BaseResponse<SubmitCashFlowResponse>> SubmitCashFlowAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId);

    [Post("/api/cash-flows/{id}/approve")]
    Task<BaseResponse<ApproveCashFlowResponse>> ApproveCashFlowAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId);

    [Post("/api/cash-flows/{id}/reject")]
    Task<BaseResponse<RejectCashFlowResponse>> RejectCashFlowAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] RejectCashFlowRequest request);

    [Get("/api/cash-flows")]
    Task<BaseResponse<GetCashFlowsResponse>> GetCashFlowsAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Query] CashFlowTypeExtended? type = null,
        [Query] CashFlowStatusExtended? status = null,
        [Query] Guid? accountId = null,
        [Query] Guid? categoryId = null,
        [Query] DateTime? startDate = null,
        [Query] DateTime? endDate = null,
        [Query] string? search = null,
        [Query] int page = 1,
        [Query] int pageSize = 20,
        [Query] string sortBy = "date",
        [Query] string sortOrder = "desc");

    [Get("/api/cash-flows/pending")]
    Task<BaseResponse<GetPendingCashFlowsResponse>> GetPendingCashFlowsAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Query] CashFlowTypeExtended? type = null,
        [Query] Guid? accountId = null);

    [Post("/api/cash-flows/from-sale")]
    Task<BaseResponse<CreateCashFlowFromSaleResponse>> CreateCashFlowFromSaleAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] CreateCashFlowFromSaleRequest request);

    [Post("/api/cash-flows/from-purchase")]
    Task<BaseResponse<CreateCashFlowFromPurchaseResponse>> CreateCashFlowFromPurchaseAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] CreateCashFlowFromPurchaseRequest request);

    [Post("/api/recurring-cash-flows")]
    Task<BaseResponse<CreateRecurringCashFlowResponse>> CreateRecurringCashFlowAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] CreateRecurringCashFlowRequest request);

    [Get("/api/recurring-cash-flows")]
    Task<BaseResponse<GetRecurringCashFlowsResponse>> GetRecurringCashFlowsAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Query] bool? isActive = true,
        [Query] CashFlowTypeExtended? type = null);

    [Put("/api/recurring-cash-flows/{id}")]
    Task<BaseResponse<UpdateRecurringCashFlowResponse>> UpdateRecurringCashFlowAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] UpdateRecurringCashFlowRequest request);

    [Patch("/api/recurring-cash-flows/{id}/toggle")]
    Task<BaseResponse<ToggleRecurringCashFlowResponse>> ToggleRecurringCashFlowAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId);

    [Post("/api/cash-flows/{id}/reconcile")]
    Task<BaseResponse<ReconcileCashFlowResponse>> ReconcileCashFlowAsync(
        Guid id,
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Body] ReconcileCashFlowRequest? request = null);

    [Get("/api/cash-flows/unreconciled")]
    Task<BaseResponse<GetUnreconciledCashFlowsResponse>> GetUnreconciledCashFlowsAsync(
        [Header("X-Application-Id")] string applicationId,
        [Header("X-Boutique-Id")] string boutiqueId,
        [Query] Guid? accountId = null,
        [Query] DateTime? startDate = null,
        [Query] DateTime? endDate = null);
}

public enum AccountType
{
    CASH = 1,           // Caisse
    BANK = 2,           // Compte bancaire
    MOBILE_MONEY = 3,    // Mobile Money,
    OTHER = 4
}


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

// Category DTOs
public enum CategoryType
{
    INCOME = 1,
    EXPENSE = 2
}

public record CreateCategoryRequest(
    string Name,
    CategoryType Type,
    string? Icon
);

public record CreateCategoryResponse(CategoryDTO Category);

public record CategoryDTO(
    Guid Id,
    string ApplicationId,
    string Name,
    CategoryType Type,
    string? Icon,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record GetCategoriesResponse(
    IReadOnlyList<CategoryDTO> Categories,
    int TotalCount
);

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
    string? RejectionReason
);

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

// CreateCashFlowFromSale DTOs
public record CreateCashFlowFromSaleRequest(
    Guid SaleId,
    string SaleReference,
    decimal Amount,
    Guid AccountId,
    string PaymentMethod,
    DateTime SaleDate,
    string? CustomerName,
    string? CustomerId,
    string CategoryId
);

public record CreateCashFlowFromSaleResponse(
    CashFlowDTO CashFlow,
    decimal NewAccountBalance
);

// CreateCashFlowFromPurchase DTOs
public record CreateCashFlowFromPurchaseRequest(
    Guid PurchaseId,
    string PurchaseReference,
    decimal Amount,
    Guid AccountId,
    string PaymentMethod,
    DateTime PurchaseDate,
    string? SupplierName,
    string? SupplierId,
    string CategoryId
);

public record CreateCashFlowFromPurchaseResponse(
    CashFlowDTO CashFlow,
    decimal NewAccountBalance
);

// RecurringCashFlow DTOs
public enum RecurringFrequency
{
    DAILY = 1,
    WEEKLY = 2,
    MONTHLY = 3,
    YEARLY = 4
}

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
