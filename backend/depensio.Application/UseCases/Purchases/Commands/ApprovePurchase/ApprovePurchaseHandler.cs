using depensio.Application.ApiExterne.Tresoreries;
using depensio.Application.Exceptions;
using depensio.Domain.Constants;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace depensio.Application.UseCases.Purchases.Commands.ApprovePurchase;

public class ApprovePurchaseHandler(
    IDepensioDbContext _depensioDbContext,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    ITresorerieService _tresorerieService,
    IBoutiqueSettingService _boutiqueSettingService,
    IGenericRepository<PurchaseStatusHistory> _purchaseStatusHistoryRepository,
    IGenericRepository<Purchase> _purchaseRepository,
    IGenericRepository<Product> _productRepository,
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

        // Check if automatic treasury transfer is enabled
        var envoiAutomatiqueEnabled = await IsEnvoiAutomatiqueEnabledAsync(command.BoutiqueId);

        // Validate required fields for Treasury call only if automatic transfer is enabled
        if (envoiAutomatiqueEnabled)
        {
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
        }

        var userId = _userContextService.GetUserId();

        Guid? cashFlowId = null;
        bool isTransferred = false;

        // Only call Treasury service if automatic transfer is enabled
        if (envoiAutomatiqueEnabled)
        {
            // AC-2, AC-3: Call ITresorerieService.CreateCashFlowFromPurchaseAsync()
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
                    isTransferred = true;
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
        }
        else
        {
            _logger.LogInformation("Automatic treasury transfer is disabled. Purchase {PurchaseId} approved without treasury transfer.", purchase.Id.Value);
        }

        // AC-1: Transition: PendingApproval (2) → Approved (3)
        var fromStatus = purchase.Status;
        purchase.Status = (int)PurchaseStatus.Approved;

        // AC-4: CashFlowId renseigné (only if transferred)
        purchase.CashFlowId = cashFlowId;

        // Mark purchase as transferred to treasury (only if automatic transfer was enabled and successful)
        purchase.IsTransferred = isTransferred;

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
            Comment = envoiAutomatiqueEnabled ? "Achat approuvé et transféré à la trésorerie" : "Achat approuvé"
        };

        // BUG FIX #503: Augmenter le stock des produits associés à l'achat approuvé
        // Seulement si le stock n'est pas en mode automatique
        var stockIsAuto = await IsStockAutomatiqueEnabledAsync(command.BoutiqueId);
        if (!stockIsAuto)
        {
            await UpdateProductStockAsync(purchase.PurchaseItems, cancellationToken);
        }

        await _purchaseStatusHistoryRepository.AddDataAsync(statusHistory, cancellationToken);
        _purchaseRepository.UpdateData(purchase);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} approved by user {UserId}, CashFlowId: {CashFlowId}, Transferred: {IsTransferred}",
            purchase.Id.Value, userId, cashFlowId, isTransferred);

        return new ApprovePurchaseResult(purchase.Id.Value, "Approved", cashFlowId);
    }

    private async Task<bool> IsEnvoiAutomatiqueEnabledAsync(Guid boutiqueId)
    {
        try
        {
            var setting = await _boutiqueSettingService.GetSettingAsync(boutiqueId, BoutiqueSettingKeys.ACHAT_KEY);
            if (setting?.Value is string json && !string.IsNullOrEmpty(json))
            {
                var settingValues = JsonSerializer.Deserialize<List<BoutiqueSettingValue>>(json);
                var envoiAutoSetting = settingValues?.FirstOrDefault(x => x.Id == BoutiqueSettingKeys.ACHAT_ENVOI_AUTOMATIQUE_TRESORERIE);
                if (envoiAutoSetting != null)
                {
                    return envoiAutoSetting.Value?.ToString()?.ToLower() == "true";
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading purchase setting for boutique {BoutiqueId}. Defaulting to false.", boutiqueId);
        }
        return false;
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

    private async Task UpdateProductStockAsync(ICollection<PurchaseItem> purchaseItems, CancellationToken cancellationToken)
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
                product.Stock += purchaseItem.Quantity;
                _productRepository.UpdateData(product);
                _logger.LogInformation("Stock updated for Product {ProductId}: +{Quantity} (new stock: {NewStock})",
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
