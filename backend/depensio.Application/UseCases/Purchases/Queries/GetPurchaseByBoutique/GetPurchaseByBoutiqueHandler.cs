using depensio.Application.UseCases.Purchases.DTOs;
using depensio.Domain.Enums;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Purchases.Queries.GetPurchaseByBoutique;
public class GetPurchaseByBoutiqueHandler(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetPurchaseByBoutiqueQuery, GetPurchaseByBoutiqueResult>
{
    public async Task<GetPurchaseByBoutiqueResult> Handle(GetPurchaseByBoutiqueQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();

        // Parse status filter
        var statusFilters = ParseStatusFilter(request.Status);

        var query = dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
                        && b.UsersBoutiques.Any(ub => ub.UserId == userId))
            .Include(b => b.Purchases)
            .ThenInclude(p => p.PurchaseItems)
            .SelectMany(b => b.Purchases);

        // Apply status filter if not "all" or empty
        if (statusFilters.Count > 0)
        {
            query = query.Where(p => statusFilters.Contains(p.Status));
        }

        var purchases = await query
            .Select(p => new PurchaseDTO
            {
                Id = p.Id.Value,
                BoutiqueId = p.BoutiqueId.Value,
                SupplierName = p.SupplierName,
                Title = p.Title,
                Description = p.Description,
                DateAchat = p.DateAchat,
                // Map int status back to string for frontend
                Status = p.Status == (int)PurchaseStatus.Draft ? "draft" :
                         p.Status == (int)PurchaseStatus.PendingApproval ? "pending_approval" :
                         p.Status == (int)PurchaseStatus.Approved ? "approved" :
                         p.Status == (int)PurchaseStatus.Rejected ? "rejected" :
                         p.Status == (int)PurchaseStatus.Cancelled ? "cancelled" : null,
                PaymentMethod = p.PaymentMethod,
                AccountId = p.AccountId,
                CategoryId = p.CategoryId,
                IsTransferred = p.IsTransferred,
                CashFlowId = p.CashFlowId,
                Items = p.PurchaseItems.Select(pi => new PurchaseItemDTO(
                    pi.Id.Value,pi.ProductId.Value, pi.Price, pi.Quantity
                )).ToList()
            })
            .OrderByDescending(p => p.DateAchat)
            .ToListAsync(cancellationToken);


        return new GetPurchaseByBoutiqueResult(purchases);
    }

    /// <summary>
    /// Parse the status filter string to a list of integer status values
    /// </summary>
    /// <param name="status">Comma-separated status string (draft,pending,approved,rejected,cancelled,all)</param>
    /// <returns>List of integer status values, empty list means no filter (all)</returns>
    private static List<int> ParseStatusFilter(string? status)
    {
        if (string.IsNullOrWhiteSpace(status) || status.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return new List<int>(); // No filter, return all
        }

        var statusValues = new List<int>();
        var statusStrings = status.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var s in statusStrings)
        {
            var statusInt = s.ToLowerInvariant() switch
            {
                "draft" => (int)PurchaseStatus.Draft,
                "pending" => (int)PurchaseStatus.PendingApproval,
                "approved" => (int)PurchaseStatus.Approved,
                "rejected" => (int)PurchaseStatus.Rejected,
                "cancelled" => (int)PurchaseStatus.Cancelled,
                "all" => -1, // Special case: if "all" is in the list, return no filter
                _ => (int?)null
            };

            if (statusInt == -1)
            {
                return new List<int>(); // "all" found, return no filter
            }

            if (statusInt.HasValue)
            {
                statusValues.Add(statusInt.Value);
            }
        }

        return statusValues;
    }
}
