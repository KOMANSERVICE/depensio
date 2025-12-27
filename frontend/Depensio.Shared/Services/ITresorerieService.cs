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

    [Post("/tresorerie/{boutiqueId}/categories")]
    Task<BaseResponse<CreateCategoryResponse>> CreateCategoryAsync(
        Guid boutiqueId,
        [Body] CreateCategoryRequest request);

    [Get("/tresorerie/{boutiqueId}/categories")]
    Task<BaseResponse<GetCategoriesResponse>> GetCategoriesAsync(
        Guid boutiqueId,
        CategoryType? type = null,
        bool includeInactive = false);

    [Post("/tresorerie/{boutiqueId}/cash-flows")]
    Task<BaseResponse<CreateCashFlowResponse>> CreateCashFlowAsync(
        Guid boutiqueId,
        [Body] CreateCashFlowRequest request);

    [Post("/tresorerie/{boutiqueId}/cash-flows/transfer")]
    Task<BaseResponse<CreateTransferResponse>> CreateTransferAsync(
        Guid boutiqueId,
        [Body] CreateTransferRequest request);

    [Put("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}")]
    Task<BaseResponse<UpdateCashFlowResponse>> UpdateCashFlowAsync(
        Guid boutiqueId,
        Guid cashFlowId,
        [Body] UpdateCashFlowRequest request);

    [Get("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}")]
    Task<BaseResponse<GetCashFlowResponse>> GetCashFlowAsync(
        Guid boutiqueId,
        Guid cashFlowId);

    [Delete("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}")]
    Task<IApiResponse> DeleteCashFlowAsync(
        Guid boutiqueId,
        Guid cashFlowId);

    [Post("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}/submit")]
    Task<BaseResponse<SubmitCashFlowResponse>> SubmitCashFlowAsync(
        Guid boutiqueId,
        Guid cashFlowId);

    [Post("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}/approve")]
    Task<BaseResponse<ApproveCashFlowResponse>> ApproveCashFlowAsync(
        Guid boutiqueId,
        Guid cashFlowId);

    [Post("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}/reject")]
    Task<BaseResponse<RejectCashFlowResponse>> RejectCashFlowAsync(
        Guid boutiqueId,
        Guid cashFlowId,
        [Body] RejectCashFlowRequest request);

    [Get("/tresorerie/{boutiqueId}/cash-flows")]
    Task<BaseResponse<GetCashFlowsResponse>> GetCashFlowsAsync(
        Guid boutiqueId,
        CashFlowTypeExtended? type = null,
        CashFlowStatusExtended? status = null,
        Guid? accountId = null,
        Guid? categoryId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        string sortBy = "date",
        string sortOrder = "desc");

    [Get("/tresorerie/{boutiqueId}/cash-flows/pending")]
    Task<BaseResponse<GetPendingCashFlowsResponse>> GetPendingCashFlowsAsync(
        Guid boutiqueId,
        CashFlowTypeExtended? type = null,
        Guid? accountId = null);

    [Post("/tresorerie/{boutiqueId}/recurring-cash-flows")]
    Task<BaseResponse<CreateRecurringCashFlowResponse>> CreateRecurringCashFlowAsync(
        Guid boutiqueId,
        [Body] CreateRecurringCashFlowRequest request);

    [Get("/tresorerie/{boutiqueId}/recurring-cash-flows")]
    Task<BaseResponse<GetRecurringCashFlowsResponse>> GetRecurringCashFlowsAsync(
        Guid boutiqueId,
        bool? isActive = true,
        CashFlowTypeExtended? type = null);

    [Put("/tresorerie/{boutiqueId}/recurring-cash-flows/{recurringCashFlowId}")]
    Task<BaseResponse<UpdateRecurringCashFlowResponse>> UpdateRecurringCashFlowAsync(
        Guid boutiqueId,
        Guid recurringCashFlowId,
        [Body] UpdateRecurringCashFlowRequest request);

    [Patch("/tresorerie/{boutiqueId}/recurring-cash-flows/{recurringCashFlowId}/toggle")]
    Task<BaseResponse<ToggleRecurringCashFlowResponse>> ToggleRecurringCashFlowAsync(
        Guid boutiqueId,
        Guid recurringCashFlowId);

    [Post("/tresorerie/{boutiqueId}/cash-flows/{cashFlowId}/reconcile")]
    Task<BaseResponse<ReconcileCashFlowResponse>> ReconcileCashFlowAsync(
        Guid boutiqueId,
        Guid cashFlowId,
        [Body] ReconcileCashFlowRequest? request = null);

    [Get("/tresorerie/{boutiqueId}/cash-flows/unreconciled")]
    Task<BaseResponse<GetUnreconciledCashFlowsResponse>> GetUnreconciledCashFlowsAsync(
        Guid boutiqueId,
        Guid? accountId = null,
        DateTime? startDate = null,
        DateTime? endDate = null);

    [Get("/tresorerie/{boutiqueId}/budgets/{budgetId}")]
    Task<BaseResponse<GetBudgetByIdResponse>> GetBudgetByIdAsync(
        Guid boutiqueId,
        Guid budgetId);

    [Get("/tresorerie/{boutiqueId}/budgets")]
    Task<BaseResponse<GetBudgetsResponse>> GetBudgetsAsync(
        Guid boutiqueId,
        bool? isActive = null,
        DateTime? startDate = null,
        DateTime? endDate = null);

    [Put("/tresorerie/{boutiqueId}/budgets/{budgetId}")]
    Task<BaseResponse<UpdateBudgetResponse>> UpdateBudgetAsync(
        Guid boutiqueId,
        Guid budgetId,
        [Body] UpdateBudgetRequest request);

    [Get("/tresorerie/{boutiqueId}/budgets/alerts")]
    Task<BaseResponse<GetBudgetAlertsResponse>> GetBudgetAlertsAsync(
        Guid boutiqueId);

    [Get("/tresorerie/{boutiqueId}/dashboard")]
    Task<BaseResponse<TreasuryDashboardDto>> GetTreasuryDashboardAsync(
        Guid boutiqueId);
}
