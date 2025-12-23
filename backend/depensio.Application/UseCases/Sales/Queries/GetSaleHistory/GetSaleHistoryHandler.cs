using depensio.Application.UseCases.Sales.DTOs;

namespace depensio.Application.UseCases.Sales.Queries.GetSaleHistory;

/// <summary>
/// Handler for GetSaleHistoryQuery
/// </summary>
public class GetSaleHistoryHandler(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetSaleHistoryQuery, GetSaleHistoryResult>
{
    public async Task<GetSaleHistoryResult> Handle(GetSaleHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        var saleId = SaleId.Of(request.SaleId);
        var boutiqueId = BoutiqueId.Of(request.BoutiqueId);

        // AC-3: Verify the user has access to this boutique and the sale exists
        var sale = await dbContext.Sales
            .Include(s => s.Boutique)
            .ThenInclude(b => b.UsersBoutiques)
            .FirstOrDefaultAsync(s =>
                s.Id == saleId &&
                s.BoutiqueId == boutiqueId &&
                s.Boutique.UsersBoutiques.Any(ub => ub.UserId == userId),
                cancellationToken);

        if (sale == null)
        {
            throw new NotFoundException($"La vente avec l'ID {request.SaleId} n'existe pas ou vous n'avez pas les droits d'accès.", nameof(request.SaleId));
        }

        // AC-1: Get the status history in chronological order (oldest first)
        var history = await dbContext.SaleStatusHistories
            .Where(h => h.SaleId == saleId)
            .OrderBy(h => h.CreatedAt) // AC-1: Liste chronologique des changements
            .Select(h => new SaleStatusHistoryDTO
            {
                Id = h.Id.Value,
                Date = h.CreatedAt, // AC-2: Date du changement
                ChangedBy = h.CreatedBy, // AC-2: Utilisateur qui a effectué le changement
                FromStatus = h.FromStatus.HasValue ? MapStatusToString(h.FromStatus.Value) : null, // AC-2: Statut avant
                ToStatus = MapStatusToString(h.ToStatus), // AC-2: Statut après
                Comment = h.Comment // AC-2: Commentaire
            })
            .ToListAsync(cancellationToken);

        return new GetSaleHistoryResult(history);
    }

    /// <summary>
    /// Maps the integer status to a user-friendly string
    /// </summary>
    private static string MapStatusToString(int status)
    {
        return status switch
        {
            (int)SaleStatus.Validated => "Validée",
            (int)SaleStatus.Cancelled => "Annulée",
            _ => "Inconnu"
        };
    }
}
