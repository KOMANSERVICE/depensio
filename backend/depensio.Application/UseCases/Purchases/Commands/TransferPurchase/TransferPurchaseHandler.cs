using depensio.Application.ApiExterne.Tresoreries;
using depensio.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Purchases.Commands.TransferPurchase;

/// <summary>
/// Handler for manually transferring an approved purchase to treasury
/// Used when a purchase is approved but the CashFlow creation failed or was not done
/// </summary>
public class TransferPurchaseHandler(
    IDepensioDbContext _depensioDbContext,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    ITresorerieService _tresorerieService,
    ILogger<TransferPurchaseHandler> _logger
    )
    : ICommandHandler<TransferPurchaseCommand, TransferPurchaseResult>
{
    public async Task<TransferPurchaseResult> Handle(
        TransferPurchaseCommand command,
        CancellationToken cancellationToken
        )
    {
        var purchaseId = PurchaseId.Of(command.PurchaseId);
        var boutiqueId = BoutiqueId.Of(command.BoutiqueId);

        // Retrieve purchase with items
        var purchase = await _depensioDbContext.Purchases
            .Include(p => p.PurchaseItems)
            .FirstOrDefaultAsync(p => p.Id == purchaseId && p.BoutiqueId == boutiqueId, cancellationToken);

        if (purchase == null)
        {
            throw new NotFoundException($"L'achat avec l'ID {command.PurchaseId} n'existe pas.", nameof(command.PurchaseId));
        }

        // Verify current status is Approved
        var currentStatus = (PurchaseStatus)purchase.Status;
        if (currentStatus != PurchaseStatus.Approved)
        {
            throw new BadRequestException($"Impossible de transférer un achat avec le statut '{currentStatus}'. Seuls les achats approuvés peuvent être transférés.");
        }

        // Check if already transferred
        if (purchase.IsTransferred)
        {
            throw new BadRequestException("Cet achat a déjà été transféré à la trésorerie.");
        }

        // Validate required fields for Treasury call
        if (!purchase.AccountId.HasValue)
        {
            throw new BadRequestException("Le compte est obligatoire pour transférer l'achat.");
        }

        if (string.IsNullOrWhiteSpace(purchase.PaymentMethod))
        {
            throw new BadRequestException("Le mode de paiement est obligatoire pour transférer l'achat.");
        }

        if (string.IsNullOrWhiteSpace(purchase.CategoryId))
        {
            throw new BadRequestException("La catégorie est obligatoire pour transférer l'achat.");
        }

        var userId = _userContextService.GetUserId();

        // Call ITresorerieService.CreateCashFlowFromPurchaseAsync()
        Guid? cashFlowId = null;
        try
        {
            var request = new CreateCashFlowFromPurchaseRequest(
                PurchaseId: purchase.Id.Value,
                PurchaseReference: $"ACH-{purchase.Id.Value.ToString()[..8].ToUpper()}",
                Amount: purchase.TotalAmount,
                AccountId: purchase.AccountId!.Value,
                PaymentMethod: purchase.PaymentMethod!,
                PurchaseDate: purchase.Date,
                SupplierName: purchase.SupplierName,
                SupplierId: null,
                CategoryId: purchase.CategoryId!
            );

            var result = await _tresorerieService.CreateCashFlowFromPurchaseAsync(
                "depensio",
                command.BoutiqueId.ToString(),
                request
            );

            if (result.Success && result.Data != null)
            {
                cashFlowId = result.Data.CashFlow.Id;
                _logger.LogInformation("CashFlow {CashFlowId} created for Purchase {PurchaseId} via manual transfer", cashFlowId, purchase.Id.Value);
            }
            else
            {
                _logger.LogError("Failed to create CashFlow for Purchase {PurchaseId} via manual transfer: {Message}", purchase.Id.Value, result.Message);
                throw new ExternalServiceException("Tresorerie", $"Échec de l'appel au service de trésorerie: {result.Message}");
            }
        }
        catch (ExternalServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating CashFlow for Purchase {PurchaseId} via manual transfer, AccountId: {AccountId}, Amount: {Amount}",
                purchase.Id.Value, purchase.AccountId, purchase.TotalAmount);
            throw new ExternalServiceException("Tresorerie", $"Erreur lors de l'appel au service de trésorerie: {ex.Message}", ex);
        }

        // Update purchase with CashFlowId and mark as transferred
        purchase.CashFlowId = cashFlowId;
        purchase.IsTransferred = true;

        // Record history entry for the transfer
        var statusHistory = new PurchaseStatusHistory
        {
            Id = PurchaseStatusHistoryId.Of(Guid.NewGuid()),
            PurchaseId = purchase.Id,
            FromStatus = (int)PurchaseStatus.Approved,
            ToStatus = (int)PurchaseStatus.Approved, // Status doesn't change
            Comment = "Transfert manuel vers la trésorerie"
        };

        _depensioDbContext.PurchaseStatusHistories.Add(statusHistory);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} manually transferred to treasury by user {UserId}, CashFlowId: {CashFlowId}",
            purchase.Id.Value, userId, cashFlowId);

        return new TransferPurchaseResult(purchase.Id.Value, "Transferred", cashFlowId);
    }
}
