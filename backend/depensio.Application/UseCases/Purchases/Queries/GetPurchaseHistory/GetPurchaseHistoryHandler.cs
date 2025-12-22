using depensio.Application.UseCases.Purchases.DTOs;

namespace depensio.Application.UseCases.Purchases.Queries.GetPurchaseHistory;

/// <summary>
/// Handler for GetPurchaseHistoryQuery
/// </summary>
public class GetPurchaseHistoryHandler(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetPurchaseHistoryQuery, GetPurchaseHistoryResult>
{
    public async Task<GetPurchaseHistoryResult> Handle(GetPurchaseHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        var purchaseId = PurchaseId.Of(request.PurchaseId);
        var boutiqueId = BoutiqueId.Of(request.BoutiqueId);

        // AC-3: Verify the user has access to this boutique and the purchase exists
        var purchase = await dbContext.Purchases
            .Include(p => p.Boutique)
            .ThenInclude(b => b.UsersBoutiques)
            .FirstOrDefaultAsync(p =>
                p.Id == purchaseId &&
                p.BoutiqueId == boutiqueId &&
                p.Boutique.UsersBoutiques.Any(ub => ub.UserId == userId),
                cancellationToken);

        if (purchase == null)
        {
            throw new NotFoundException($"L'achat avec l'ID {request.PurchaseId} n'existe pas ou vous n'avez pas les droits d'accès.", nameof(request.PurchaseId));
        }

        // AC-1: Get the status history in chronological order (oldest first)
        var history = await dbContext.PurchaseStatusHistories
            .Where(h => h.PurchaseId == purchaseId)
            .OrderBy(h => h.CreatedAt) // AC-1: Liste chronologique des changements
            .Select(h => new PurchaseStatusHistoryDTO
            {
                Id = h.Id.Value,
                Date = h.CreatedAt, // AC-2: Date du changement
                ChangedBy = h.CreatedBy, // AC-2: Utilisateur qui a effectué le changement
                FromStatus = h.FromStatus.HasValue ? MapStatusToString(h.FromStatus.Value) : null, // AC-2: Statut avant
                ToStatus = MapStatusToString(h.ToStatus), // AC-2: Statut après
                Comment = h.Comment // AC-2: Commentaire
            })
            .ToListAsync(cancellationToken);

        return new GetPurchaseHistoryResult(history);
    }

    /// <summary>
    /// Maps the integer status to a user-friendly string
    /// </summary>
    private static string MapStatusToString(int status)
    {
        return status switch
        {
            (int)PurchaseStatus.Draft => "Brouillon",
            (int)PurchaseStatus.PendingApproval => "En attente de validation",
            (int)PurchaseStatus.Approved => "Approuvé",
            (int)PurchaseStatus.Rejected => "Rejeté",
            (int)PurchaseStatus.Cancelled => "Annulé",
            _ => "Inconnu"
        };
    }
}
