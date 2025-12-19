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
}

public enum AccountType
{
    Cash = 0,
    Bank = 1,
    MobileMoney = 2,
    Other = 3
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

// Category DTOs
public enum CategoryType
{
    Income = 0,
    Expense = 1
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
    decimal DestinationAccountBalance
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
