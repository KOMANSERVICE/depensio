using depensio.Application.UseCases.Purchases.DTOs;

namespace depensio.Application.UseCases.Purchases.Queries.GetPurchaseHistory;

/// <summary>
/// Query to get the status history of a purchase
/// </summary>
/// <param name="PurchaseId">The purchase ID</param>
/// <param name="BoutiqueId">The boutique ID for authorization</param>
public record GetPurchaseHistoryQuery(Guid PurchaseId, Guid BoutiqueId)
    : IQuery<GetPurchaseHistoryResult>;

/// <summary>
/// Result containing the list of status history entries
/// </summary>
/// <param name="History">List of status history entries ordered chronologically (oldest first)</param>
public record GetPurchaseHistoryResult(IEnumerable<PurchaseStatusHistoryDTO> History);
