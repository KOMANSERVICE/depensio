using depensio.Application.UseCases.Sales.DTOs;
using depensio.Domain.Enums;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Sales.Queries.GetSaleByBoutique;

public class GetSaleByBoutiqueHandler(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetSaleByBoutiqueQuery, GetSaleByBoutiqueResult>
{
    public async Task<GetSaleByBoutiqueResult> Handle(GetSaleByBoutiqueQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();

        // Parse status filter
        var statusFilters = ParseStatusFilter(request.Status);

        var query = dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId))
            .Include(b => b.Sales)
            .SelectMany(b => b.Sales);

        // Apply status filter if not "all" or empty
        if (statusFilters.Count > 0)
        {
            query = query.Where(p => statusFilters.Contains((int)p.Status));
        }

        var sales = await query
            .Select(p => new SaleDTO
            {
                Id = p.Id.Value,
                BoutiqueId = p.BoutiqueId.Value,
                Date = p.Date,
                Status = (int)p.Status,
                CancelledAt = p.CancelledAt,
                CancellationReason = p.CancellationReason,
                CashFlowId = p.CashFlowId,
                Items = p.SaleItems.Select(i => new SaleItemDTO(i.Id.Value, i.ProductId.Value, i.Price, i.Quantity, new List<string>())).ToList(),
            })
            .OrderByDescending(p => p.Date)
            .ToListAsync(cancellationToken);

        return new GetSaleByBoutiqueResult(sales);
    }

    /// <summary>
    /// Parse the status filter string to a list of integer status values
    /// </summary>
    /// <param name="status">Status string (validated, cancelled, all)</param>
    /// <returns>List of integer status values, empty list means no filter (all)</returns>
    private static List<int> ParseStatusFilter(string? status)
    {
        if (string.IsNullOrWhiteSpace(status) || status.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return new List<int>(); // No filter, return all
        }

        return status.ToLowerInvariant() switch
        {
            "validated" => new List<int> { (int)SaleStatus.Validated },
            "cancelled" => new List<int> { (int)SaleStatus.Cancelled },
            _ => new List<int>() // Unknown status, return all
        };
    }
}
