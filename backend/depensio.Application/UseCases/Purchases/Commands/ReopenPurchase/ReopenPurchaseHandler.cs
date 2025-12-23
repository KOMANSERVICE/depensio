using depensio.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Purchases.Commands.ReopenPurchase;

/// <summary>
/// Handler for reopening a rejected purchase
/// AC-1: Transition: Rejected (4) -> Draft (1)
/// AC-2: L'achat redevient modifiable
/// AC-3: RejectionReason precedent conserve dans l'historique
/// AC-4: Peut ensuite suivre le flux normal (modifier -> soumettre -> approuver)
/// AC-5: Historique: FromStatus = 4, ToStatus = 1
/// </summary>
public class ReopenPurchaseHandler(
    IDepensioDbContext _depensioDbContext,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    IGenericRepository<PurchaseStatusHistory> _purchaseStatusHistoryRepository,
    IGenericRepository<Purchase> _purchaseRepository,
    ILogger<ReopenPurchaseHandler> _logger
    )
    : ICommandHandler<ReopenPurchaseCommand, ReopenPurchaseResult>
{
    public async Task<ReopenPurchaseResult> Handle(
        ReopenPurchaseCommand command,
        CancellationToken cancellationToken
        )
    {
        var purchaseId = PurchaseId.Of(command.PurchaseId);
        var boutiqueId = BoutiqueId.Of(command.BoutiqueId);

        // Retrieve purchase
        var purchase = await _depensioDbContext.Purchases
            .FirstOrDefaultAsync(p => p.Id == purchaseId && p.BoutiqueId == boutiqueId, cancellationToken);

        if (purchase == null)
        {
            throw new NotFoundException($"L'achat avec l'ID {command.PurchaseId} n'existe pas.", nameof(command.PurchaseId));
        }

        // AC-1: Verify current status is Rejected (4)
        var currentStatus = (PurchaseStatus)purchase.Status;
        if (currentStatus != PurchaseStatus.Rejected)
        {
            throw new BadRequestException($"Impossible de rouvrir un achat avec le statut '{currentStatus}'. Seuls les achats rejetés peuvent être rouverts.");
        }

        var userId = _userContextService.GetUserId();

        // AC-3: Store the rejection reason in history before clearing it
        var previousRejectionReason = purchase.RejectionReason;

        // AC-1: Transition: Rejected (4) -> Draft (1)
        var fromStatus = purchase.Status;
        purchase.Status = (int)PurchaseStatus.Draft;

        // AC-2: L'achat redevient modifiable - clear rejection reason from the purchase
        // The rejection reason is preserved in the status history
        purchase.RejectionReason = null;

        // AC-5: Historique: FromStatus = 4, ToStatus = 1
        // AC-3: RejectionReason precedent conserve dans l'historique (as comment)
        var statusHistory = new PurchaseStatusHistory
        {
            Id = PurchaseStatusHistoryId.Of(Guid.NewGuid()),
            PurchaseId = purchase.Id,
            FromStatus = fromStatus, // AC-5: FromStatus = 4 (Rejected)
            ToStatus = (int)PurchaseStatus.Draft, // AC-5: ToStatus = 1 (Draft)
            Comment = string.IsNullOrEmpty(previousRejectionReason)
                ? "Achat rouvert pour correction"
                : $"Achat rouvert pour correction. Motif de rejet précédent: {previousRejectionReason}"
        };


        await _purchaseStatusHistoryRepository.AddDataAsync(statusHistory, cancellationToken);
        _purchaseRepository.UpdateData(purchase);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} reopened by user {UserId}. Previous rejection reason: {RejectionReason}",
            purchase.Id.Value, userId, previousRejectionReason);

        // AC-4: Peut ensuite suivre le flux normal (handled by UpdatePurchaseHandler, SubmitPurchaseHandler, etc.)

        return new ReopenPurchaseResult(purchase.Id.Value, "Draft");
    }
}
