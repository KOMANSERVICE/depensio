using depensio.Application.ApiExterne.Tresoreries;
using depensio.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Purchases.Commands.ApprovePurchase;

public class ApprovePurchaseHandler(
    IDepensioDbContext _depensioDbContext,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    ITresorerieService _tresorerieService,
    ILogger<ApprovePurchaseHandler> _logger
    )
    : ICommandHandler<ApprovePurchaseCommand, ApprovePurchaseResult>
{
    public async Task<ApprovePurchaseResult> Handle(
        ApprovePurchaseCommand command,
        CancellationToken cancellationToken
        )
    {
        var purchaseId = PurchaseId.Of(command.PurchaseId);
        var boutiqueId = BoutiqueId.Of(command.BoutiqueId);

        // AC-1: Retrieve purchase with items
        var purchase = await _depensioDbContext.Purchases
            .Include(p => p.PurchaseItems)
            .FirstOrDefaultAsync(p => p.Id == purchaseId && p.BoutiqueId == boutiqueId, cancellationToken);

        if (purchase == null)
        {
            throw new NotFoundException($"L'achat avec l'ID {command.PurchaseId} n'existe pas.", nameof(command.PurchaseId));
        }

        // AC-1: Verify current status is PendingApproval (2)
        var currentStatus = (PurchaseStatus)purchase.Status;
        if (currentStatus != PurchaseStatus.PendingApproval)
        {
            throw new BadRequestException($"Impossible d'approuver un achat avec le statut '{currentStatus}'. Seuls les achats en attente de validation peuvent être approuvés.");
        }

        // Validate required fields for Treasury call
        if (!purchase.AccountId.HasValue)
        {
            throw new BadRequestException("Le compte est obligatoire pour approuver l'achat.");
        }

        if (string.IsNullOrWhiteSpace(purchase.PaymentMethod))
        {
            throw new BadRequestException("Le mode de paiement est obligatoire pour approuver l'achat.");
        }

        if (string.IsNullOrWhiteSpace(purchase.CategoryId))
        {
            throw new BadRequestException("La catégorie est obligatoire pour approuver l'achat.");
        }

        var userId = _userContextService.GetUserId();

        // AC-2, AC-3: Call ITresorerieService.CreateCashFlowFromPurchaseAsync()
        Guid? cashFlowId = null;
        try
        {
            var request = new CreateCashFlowFromPurchaseRequest(
                PurchaseId: purchase.Id.Value,
                PurchaseReference: $"ACH-{purchase.Id.Value.ToString()[..8].ToUpper()}",
                Amount: purchase.TotalAmount, // AC-3: TotalAmount
                AccountId: purchase.AccountId!.Value, // AC-3: AccountId
                PaymentMethod: purchase.PaymentMethod!, // AC-3: PaymentMethodId
                PurchaseDate: purchase.Date,
                SupplierName: purchase.SupplierName,
                SupplierId: null,
                CategoryId: purchase.CategoryId! // AC-3: CategoryId (part of reference data)
            );

            var result = await _tresorerieService.CreateCashFlowFromPurchaseAsync(
                "depensio",
                command.BoutiqueId.ToString(),
                request
            );

            if (result.Success && result.Data != null)
            {
                // AC-4: CashFlowId renseigné avec l'ID retourné par le service
                cashFlowId = result.Data.CashFlow.Id;
                _logger.LogInformation("CashFlow {CashFlowId} created for Purchase {PurchaseId}", cashFlowId, purchase.Id.Value);
            }
            else
            {
                // Gestion d'erreur Trésorerie: retourner erreur 502 avec message explicite
                _logger.LogError("Failed to create CashFlow for Purchase {PurchaseId}: {Message}", purchase.Id.Value, result.Message);
                throw new ExternalServiceException("Tresorerie", $"Échec de l'appel au service de trésorerie: {result.Message}");
            }
        }
        catch (ExternalServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Gestion d'erreur Trésorerie: L'achat reste en PendingApproval (2)
            _logger.LogError(ex, "Error creating CashFlow for Purchase {PurchaseId}, AccountId: {AccountId}, Amount: {Amount}",
                purchase.Id.Value, purchase.AccountId, purchase.TotalAmount);
            throw new ExternalServiceException("Tresorerie", $"Erreur lors de l'appel au service de trésorerie: {ex.Message}", ex);
        }

        // AC-1: Transition: PendingApproval (2) → Approved (3)
        var fromStatus = purchase.Status;
        purchase.Status = (int)PurchaseStatus.Approved;

        // AC-4: CashFlowId renseigné
        purchase.CashFlowId = cashFlowId;

        // AC-5: ApprovedAt = DateTime.UtcNow
        purchase.ApprovedAt = DateTime.UtcNow;

        // AC-6: ApprovedBy = CurrentUserId
        purchase.ApprovedBy = userId;

        // AC-7: Historique: FromStatus = 2, ToStatus = 3
        var statusHistory = new PurchaseStatusHistory
        {
            Id = PurchaseStatusHistoryId.Of(Guid.NewGuid()),
            PurchaseId = purchase.Id,
            FromStatus = fromStatus, // AC-7: FromStatus = 2 (PendingApproval)
            ToStatus = (int)PurchaseStatus.Approved, // AC-7: ToStatus = 3 (Approved)
            Comment = "Achat approuvé"
        };

        _depensioDbContext.PurchaseStatusHistories.Add(statusHistory);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} approved by user {UserId}, CashFlowId: {CashFlowId}",
            purchase.Id.Value, userId, cashFlowId);

        return new ApprovePurchaseResult(purchase.Id.Value, "Approved", cashFlowId);
    }
}
