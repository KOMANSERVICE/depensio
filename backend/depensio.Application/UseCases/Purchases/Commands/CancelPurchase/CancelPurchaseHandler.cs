using depensio.Application.ApiExterne.Tresoreries;
using depensio.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Purchases.Commands.CancelPurchase;

/// <summary>
/// Handler for cancelling a purchase
/// US-PUR-008: Approved -> Cancelled (with Tresorerie call if CashFlowId exists)
/// US-PUR-009: Draft/PendingApproval/Rejected -> Cancelled (no Tresorerie call)
///
/// AC-1 (US-PUR-008): Transition: Approved (3) -> Cancelled (5)
/// AC-1 (US-PUR-009): Transitions: Draft (1) -> Cancelled (5), PendingApproval (2) -> Cancelled (5), Rejected (4) -> Cancelled (5)
/// AC-2 (US-PUR-008): RejectionReason obligatoire pour Approved
/// AC-2 (US-PUR-009): RejectionReason optionnel pour Draft/PendingApproval/Rejected
/// AC-3 (US-PUR-008): Si CashFlowId non null -> Appel annulation/contre-passation Tresorerie
/// AC-3 (US-PUR-009): Aucun appel Tresorerie (pas de CashFlow)
/// AC-4: Les PurchaseItems restent intacts (audit)
/// AC-5: Historique enregistré
/// AC-6: Etat final - aucune transition possible depuis Cancelled
/// </summary>
public class CancelPurchaseHandler(
    IDepensioDbContext _depensioDbContext,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    ITresorerieService _tresorerieService,
    ILogger<CancelPurchaseHandler> _logger
    )
    : ICommandHandler<CancelPurchaseCommand, CancelPurchaseResult>
{
    // Statuts autorisés pour l'annulation
    private static readonly PurchaseStatus[] AllowedStatuses =
    [
        PurchaseStatus.Draft,           // US-PUR-009
        PurchaseStatus.PendingApproval, // US-PUR-009
        PurchaseStatus.Rejected,        // US-PUR-009
        PurchaseStatus.Approved         // US-PUR-008
    ];

    public async Task<CancelPurchaseResult> Handle(
        CancelPurchaseCommand command,
        CancellationToken cancellationToken
        )
    {
        var purchaseId = PurchaseId.Of(command.PurchaseId);
        var boutiqueId = BoutiqueId.Of(command.BoutiqueId);

        // Retrieve purchase (AC-4: PurchaseItems are not touched)
        var purchase = await _depensioDbContext.Purchases
            .FirstOrDefaultAsync(p => p.Id == purchaseId && p.BoutiqueId == boutiqueId, cancellationToken);

        if (purchase == null)
        {
            throw new NotFoundException($"L'achat avec l'ID {command.PurchaseId} n'existe pas.", nameof(command.PurchaseId));
        }

        var currentStatus = (PurchaseStatus)purchase.Status;

        // AC-1: Vérifier que le statut actuel permet l'annulation
        if (!AllowedStatuses.Contains(currentStatus))
        {
            throw new BadRequestException($"Impossible d'annuler un achat avec le statut '{currentStatus}'. Seuls les achats en brouillon, en attente d'approbation, rejetés ou approuvés peuvent être annulés.");
        }

        // AC-2 (US-PUR-008): RejectionReason obligatoire pour Approved
        if (currentStatus == PurchaseStatus.Approved && string.IsNullOrWhiteSpace(command.Reason))
        {
            throw new BadRequestException("Le motif d'annulation est obligatoire pour un achat approuvé.");
        }

        var userId = _userContextService.GetUserId();

        // AC-3 (US-PUR-008): Si CashFlowId non null -> Appel annulation/contre-passation Tresorerie
        // AC-3 (US-PUR-009): Aucun appel Tresorerie pour Draft/PendingApproval/Rejected
        if (currentStatus == PurchaseStatus.Approved && purchase.CashFlowId.HasValue)
        {
            try
            {
                var response = await _tresorerieService.DeleteCashFlowAsync(
                    purchase.CashFlowId.Value,
                    "depensio",
                    command.BoutiqueId.ToString()
                );

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to delete CashFlow {CashFlowId} for Purchase {PurchaseId}",
                        purchase.CashFlowId.Value, purchase.Id.Value);
                    throw new ExternalServiceException("Tresorerie",
                        $"Échec de l'annulation du mouvement de trésorerie associé. Veuillez réessayer.");
                }

                _logger.LogInformation("CashFlow {CashFlowId} deleted for Purchase {PurchaseId}",
                    purchase.CashFlowId.Value, purchase.Id.Value);
            }
            catch (ExternalServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CashFlow {CashFlowId} for Purchase {PurchaseId}",
                    purchase.CashFlowId, purchase.Id.Value);
                throw new ExternalServiceException("Tresorerie",
                    $"Erreur lors de l'annulation du mouvement de trésorerie: {ex.Message}", ex);
            }
        }

        // Transition vers Cancelled
        var fromStatus = purchase.Status;
        purchase.Status = (int)PurchaseStatus.Cancelled;

        // Store cancellation reason in RejectionReason field (optionnel pour US-PUR-009)
        if (!string.IsNullOrWhiteSpace(command.Reason))
        {
            purchase.RejectionReason = command.Reason;
        }

        // Clear CashFlowId since the CashFlow has been deleted (si applicable)
        if (purchase.CashFlowId.HasValue)
        {
            purchase.CashFlowId = null;
            purchase.IsTransferred = false;
        }

        // AC-5: Historique enregistré
        var statusHistory = new PurchaseStatusHistory
        {
            Id = PurchaseStatusHistoryId.Of(Guid.NewGuid()),
            PurchaseId = purchase.Id,
            FromStatus = fromStatus,
            ToStatus = (int)PurchaseStatus.Cancelled,
            Comment = command.Reason // Peut être null pour US-PUR-009
        };

        _depensioDbContext.PurchaseStatusHistories.Add(statusHistory);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} cancelled from status {FromStatus} by user {UserId}. Reason: {Reason}",
            purchase.Id.Value, currentStatus, userId, command.Reason ?? "(aucun motif)");

        // AC-6: Etat final - aucune transition possible depuis Cancelled
        // This is enforced by other handlers checking the current status

        return new CancelPurchaseResult(purchase.Id.Value, "Cancelled");
    }
}
