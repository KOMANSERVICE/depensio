using depensio.Application.ApiExterne.Tresoreries;
using depensio.Application.Exceptions;
using depensio.Domain.Constants;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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
    IBoutiqueSettingService _boutiqueSettingService,
    IGenericRepository<PurchaseStatusHistory> _purchaseStatusHistoryRepository,
    IGenericRepository<Purchase> _purchaseRepository,
    IGenericRepository<Product> _productRepository,
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

        // Retrieve purchase with items (AC-4: PurchaseItems are not touched, but we need them for stock reversion)
        var purchase = await _depensioDbContext.Purchases
            .Include(p => p.PurchaseItems)
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

        // AC-3 (US-PUR-008): Si CashFlowId non null -> Appel contre-passation Tresorerie (US-TRS-003)
        // AC-3 (US-PUR-009): Aucun appel Tresorerie pour Draft/PendingApproval/Rejected
        Guid? reversalCashFlowId = null;
        if (currentStatus == PurchaseStatus.Approved && purchase.CashFlowId.HasValue)
        {
            try
            {
                var reverseRequest = new ReverseCashFlowRequest(
                    Reason: command.Reason ?? "Annulation achat",
                    SourceType: "Purchase",
                    SourceId: purchase.Id.Value
                );

                var response = await _tresorerieService.ReverseCashFlowAsync(
                    purchase.CashFlowId.Value,
                    "depensio",
                    command.BoutiqueId.ToString(),
                    reverseRequest
                );

                if (!response.Success || response.Data == null)
                {
                    _logger.LogError("Failed to reverse CashFlow {CashFlowId} for Purchase {PurchaseId}. Message: {Message}",
                        purchase.CashFlowId.Value, purchase.Id.Value, response.Message);
                    throw new ExternalServiceException("Tresorerie",
                        $"Échec de la contre-passation du mouvement de trésorerie associé. Veuillez réessayer.");
                }

                reversalCashFlowId = response.Data.ReversalCashFlowId;
                _logger.LogInformation("CashFlow {CashFlowId} reversed for Purchase {PurchaseId}. Reversal CashFlowId: {ReversalCashFlowId}",
                    purchase.CashFlowId.Value, purchase.Id.Value, reversalCashFlowId);
            }
            catch (ExternalServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reversing CashFlow {CashFlowId} for Purchase {PurchaseId}",
                    purchase.CashFlowId, purchase.Id.Value);
                throw new ExternalServiceException("Tresorerie",
                    $"Erreur lors de la contre-passation du mouvement de trésorerie: {ex.Message}", ex);
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

        // Store the reversal CashFlowId for audit trail (contre-passation)
        // Keep the original CashFlowId for reference (audit trail)
        if (reversalCashFlowId.HasValue)
        {
            purchase.ReversalCashFlowId = reversalCashFlowId;
        }

        // BUG FIX #503: Diminuer le stock des produits si l'achat approuvé est annulé
        // Seulement si le stock n'est pas en mode automatique
        if (currentStatus == PurchaseStatus.Approved)
        {
            var stockIsAuto = await IsStockAutomatiqueEnabledAsync(command.BoutiqueId);
            if (!stockIsAuto)
            {
                await RevertProductStockAsync(purchase.PurchaseItems, cancellationToken);
            }
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

        await _purchaseStatusHistoryRepository.AddDataAsync(statusHistory, cancellationToken);
        _purchaseRepository.UpdateData(purchase);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} cancelled from status {FromStatus} by user {UserId}. Reason: {Reason}",
            purchase.Id.Value, currentStatus, userId, command.Reason ?? "(aucun motif)");

        // AC-6: Etat final - aucune transition possible depuis Cancelled
        // This is enforced by other handlers checking the current status

        return new CancelPurchaseResult(purchase.Id.Value, "Cancelled");
    }

    private async Task<bool> IsStockAutomatiqueEnabledAsync(Guid boutiqueId)
    {
        try
        {
            var setting = await _boutiqueSettingService.GetSettingAsync(boutiqueId, BoutiqueSettingKeys.PRODUCT_KEY);
            if (setting?.Value is string json && !string.IsNullOrEmpty(json))
            {
                var settingValues = JsonSerializer.Deserialize<List<BoutiqueSettingValue>>(json);
                var stockAutoSetting = settingValues?.FirstOrDefault(x => x.Id == BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE);
                if (stockAutoSetting != null)
                {
                    return stockAutoSetting.Value?.ToString()?.ToLower() == "true";
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading product setting for boutique {BoutiqueId}. Defaulting to false.", boutiqueId);
        }
        return false;
    }

    private async Task RevertProductStockAsync(ICollection<PurchaseItem> purchaseItems, CancellationToken cancellationToken)
    {
        var productIds = purchaseItems.Select(pi => pi.ProductId).ToList();
        var products = await _depensioDbContext.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        foreach (var purchaseItem in purchaseItems)
        {
            var product = products.FirstOrDefault(p => p.Id == purchaseItem.ProductId);
            if (product != null)
            {
                product.Stock -= purchaseItem.Quantity;
                _productRepository.UpdateData(product);
                _logger.LogInformation("Stock reverted for Product {ProductId}: -{Quantity} (new stock: {NewStock})",
                    product.Id.Value, purchaseItem.Quantity, product.Stock);
            }
        }
    }
}

internal class BoutiqueSettingValue
{
    public string Id { get; set; } = string.Empty;
    public string LabelValue { get; set; } = string.Empty;
    public object? Value { get; set; }
    public string LabelText { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
