using depensio.Application.UseCases.Sales.DTOs;

namespace depensio.Application.UseCases.Sales.Queries.GetSaleByBoutique;

/// <summary>
/// Query to get sales by boutique with optional status filter
/// </summary>
/// <param name="BoutiqueId">The boutique ID</param>
/// <param name="Status">Optional status filter (validated, cancelled, all). Default: all</param>
public record GetSaleByBoutiqueQuery(Guid BoutiqueId, string? Status = null)
    : IQuery<GetSaleByBoutiqueResult>;

public record GetSaleByBoutiqueResult(IEnumerable<SaleDTO> Sales);
