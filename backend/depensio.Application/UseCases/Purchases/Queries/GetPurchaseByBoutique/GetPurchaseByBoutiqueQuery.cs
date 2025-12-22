using depensio.Application.UseCases.Purchases.DTOs;

namespace depensio.Application.UseCases.Purchases.Queries.GetPurchaseByBoutique;

/// <summary>
/// Query to get purchases by boutique with optional status filter
/// </summary>
/// <param name="BoutiqueId">The boutique ID</param>
/// <param name="Status">Optional comma-separated status filter (draft,pending,approved,rejected,cancelled,all). Default: all</param>
public record GetPurchaseByBoutiqueQuery(Guid BoutiqueId, string? Status = null)
    : IQuery<GetPurchaseByBoutiqueResult>;

public record GetPurchaseByBoutiqueResult(IEnumerable<PurchaseDTO> Purchases);
