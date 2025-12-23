using depensio.Application.UseCases.Sales.DTOs;

namespace depensio.Application.UseCases.Sales.Queries.GetSaleHistory;

/// <summary>
/// Query to get the status history of a sale
/// </summary>
/// <param name="SaleId">The sale ID</param>
/// <param name="BoutiqueId">The boutique ID for authorization</param>
public record GetSaleHistoryQuery(Guid SaleId, Guid BoutiqueId)
    : IQuery<GetSaleHistoryResult>;

/// <summary>
/// Result containing the list of status history entries
/// </summary>
/// <param name="History">List of status history entries ordered chronologically (oldest first)</param>
public record GetSaleHistoryResult(IEnumerable<SaleStatusHistoryDTO> History);
