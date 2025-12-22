using depensio.Application.ApiExterne.Tresoreries;
using depensio.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Purchases.Commands.CancelPurchase;

/// <summary>
/// Handler for cancelling an approved purchase
/// AC-1: Transition: Approved (3) -> Cancelled (5)
/// AC-2: RejectionReason obligatoire (motif d'annulation) - validated by command validator
/// AC-3: Si CashFlowId non null -> Appel annulation/contre-passation Tresorerie
/// AC-4: Les PurchaseItems restent intacts (audit)
/// AC-5: Historique: FromStatus = 3, ToStatus = 5, Comment = RejectionReason
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

        // AC-1: Verify current status is Approved (3)
        var currentStatus = (PurchaseStatus)purchase.Status;
        if (currentStatus != PurchaseStatus.Approved)
        {
            throw new BadRequestException($"Impossible d'annuler un achat avec le statut '{currentStatus}'. Seuls les achats approuvés peuvent être annulés.");
        }

        var userId = _userContextService.GetUserId();

        // AC-3: Si CashFlowId non null -> Appel annulation/contre-passation Tresorerie
        if (purchase.CashFlowId.HasValue)
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

        // AC-1: Transition: Approved (3) -> Cancelled (5)
        var fromStatus = purchase.Status;
        purchase.Status = (int)PurchaseStatus.Cancelled;

        // Store cancellation reason in RejectionReason field
        purchase.RejectionReason = command.Reason;

        // Clear CashFlowId since the CashFlow has been deleted
        purchase.CashFlowId = null;

        // AC-5: Historique: FromStatus = 3, ToStatus = 5, Comment = RejectionReason
        var statusHistory = new PurchaseStatusHistory
        {
            Id = PurchaseStatusHistoryId.Of(Guid.NewGuid()),
            PurchaseId = purchase.Id,
            FromStatus = fromStatus, // AC-5: FromStatus = 3 (Approved)
            ToStatus = (int)PurchaseStatus.Cancelled, // AC-5: ToStatus = 5 (Cancelled)
            Comment = command.Reason // AC-5: Comment = RejectionReason
        };

        _depensioDbContext.PurchaseStatusHistories.Add(statusHistory);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} cancelled by user {UserId}. Reason: {Reason}",
            purchase.Id.Value, userId, command.Reason);

        // AC-6: Etat final - aucune transition possible depuis Cancelled
        // This is enforced by other handlers checking the current status

        return new CancelPurchaseResult(purchase.Id.Value, "Cancelled");
    }
}
