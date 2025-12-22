using depensio.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Purchases.Commands.RejectPurchase;

/// <summary>
/// Handler for rejecting a purchase pending approval
/// AC-1: Transition: PendingApproval (2) -> Rejected (4)
/// AC-2: RejectionReason obligatoire (min 10 caracteres) - validated by command validator
/// AC-3: Aucun appel au service Tresorerie
/// AC-4: Historique: FromStatus = 2, ToStatus = 4, Comment = RejectionReason
/// AC-5: L'achat peut ensuite etre modifie et resoumis
/// </summary>
public class RejectPurchaseHandler(
    IDepensioDbContext _depensioDbContext,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    ILogger<RejectPurchaseHandler> _logger
    )
    : ICommandHandler<RejectPurchaseCommand, RejectPurchaseResult>
{
    public async Task<RejectPurchaseResult> Handle(
        RejectPurchaseCommand command,
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

        // AC-1: Verify current status is PendingApproval (2)
        var currentStatus = (PurchaseStatus)purchase.Status;
        if (currentStatus != PurchaseStatus.PendingApproval)
        {
            throw new BadRequestException($"Impossible de rejeter un achat avec le statut '{currentStatus}'. Seuls les achats en attente de validation peuvent être rejetés.");
        }

        var userId = _userContextService.GetUserId();

        // AC-3: Aucun appel au service Tresorerie (no external service call needed)

        // AC-1: Transition: PendingApproval (2) -> Rejected (4)
        var fromStatus = purchase.Status;
        purchase.Status = (int)PurchaseStatus.Rejected;

        // Store rejection reason
        purchase.RejectionReason = command.RejectionReason;

        // AC-4: Historique: FromStatus = 2, ToStatus = 4, Comment = RejectionReason
        var statusHistory = new PurchaseStatusHistory
        {
            Id = PurchaseStatusHistoryId.Of(Guid.NewGuid()),
            PurchaseId = purchase.Id,
            FromStatus = fromStatus, // AC-4: FromStatus = 2 (PendingApproval)
            ToStatus = (int)PurchaseStatus.Rejected, // AC-4: ToStatus = 4 (Rejected)
            Comment = command.RejectionReason // AC-4: Comment = RejectionReason
        };

        _depensioDbContext.PurchaseStatusHistories.Add(statusHistory);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} rejected by user {UserId}. Reason: {RejectionReason}",
            purchase.Id.Value, userId, command.RejectionReason);

        // AC-5: L'achat peut ensuite etre modifie et resoumis (handled by UpdatePurchaseHandler which allows Draft/Rejected status)

        return new RejectPurchaseResult(purchase.Id.Value, "Rejected");
    }
}
