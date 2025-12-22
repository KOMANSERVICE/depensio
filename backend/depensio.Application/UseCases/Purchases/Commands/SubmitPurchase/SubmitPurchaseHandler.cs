using depensio.Application.ApiExterne.Tresoreries;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Purchases.Commands.SubmitPurchase;

public class SubmitPurchaseHandler(
    IDepensioDbContext _depensioDbContext,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    ITresorerieService _tresorerieService,
    ILogger<SubmitPurchaseHandler> _logger
    )
    : ICommandHandler<SubmitPurchaseCommand, SubmitPurchaseResult>
{
    public async Task<SubmitPurchaseResult> Handle(
        SubmitPurchaseCommand command,
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

        // AC-1: Verify current status is Draft
        var currentStatus = (PurchaseStatus)purchase.Status;
        if (currentStatus != PurchaseStatus.Draft)
        {
            throw new BadRequestException($"Impossible de soumettre un achat avec le statut '{currentStatus}'. Seuls les achats en brouillon peuvent être soumis.");
        }

        // AC-5: Validate at least 1 PurchaseItem
        if (purchase.PurchaseItems == null || !purchase.PurchaseItems.Any())
        {
            throw new BadRequestException("L'achat doit contenir au moins un article pour être soumis.");
        }

        // AC-2: Validate PaymentMethod is required
        if (string.IsNullOrWhiteSpace(purchase.PaymentMethod))
        {
            throw new BadRequestException("Le mode de paiement est obligatoire pour soumettre l'achat.");
        }

        // AC-3: Validate AccountId is required
        if (!purchase.AccountId.HasValue)
        {
            throw new BadRequestException("Le compte est obligatoire pour soumettre l'achat.");
        }

        // AC-4: Validate CategoryId is required
        if (string.IsNullOrWhiteSpace(purchase.CategoryId))
        {
            throw new BadRequestException("La catégorie est obligatoire pour soumettre l'achat.");
        }

        // AC-6, AC-7, AC-8: Validate references exist in Treasury API
        await ValidateTreasuryReferencesAsync(command.BoutiqueId, purchase.AccountId.Value, purchase.CategoryId, cancellationToken);

        var userId = _userContextService.GetUserId();

        // AC-1: Transition Draft (1) -> PendingApproval (2)
        var fromStatus = purchase.Status;
        purchase.Status = (int)PurchaseStatus.PendingApproval;

        // AC-10: Create status history entry
        var statusHistory = new PurchaseStatusHistory
        {
            Id = PurchaseStatusHistoryId.Of(Guid.NewGuid()),
            PurchaseId = purchase.Id,
            FromStatus = fromStatus, // AC-10: FromStatus = 1 (Draft)
            ToStatus = (int)PurchaseStatus.PendingApproval, // AC-10: ToStatus = 2 (PendingApproval)
            Comment = "Achat soumis pour validation"
        };

        _depensioDbContext.PurchaseStatusHistories.Add(statusHistory);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} submitted for approval by user {UserId}", purchase.Id.Value, userId);

        return new SubmitPurchaseResult(purchase.Id.Value, "PendingApproval");
    }

    /// <summary>
    /// AC-6, AC-7, AC-8: Validate that AccountId and CategoryId exist in Treasury API
    /// </summary>
    private async Task ValidateTreasuryReferencesAsync(Guid boutiqueId, Guid accountId, string categoryId, CancellationToken cancellationToken)
    {
        // AC-7: Validate AccountId exists and is active
        try
        {
            var accountsResponse = await _tresorerieService.GetAccountsAsync(
                "depensio",
                boutiqueId.ToString(),
                includeInactive: false);

            if (accountsResponse.Success && accountsResponse.Data != null)
            {
                var accountExists = accountsResponse.Data.Accounts.Any(a => a.Id == accountId && a.IsActive);
                if (!accountExists)
                {
                    throw new BadRequestException($"Le compte avec l'ID {accountId} n'existe pas ou n'est pas actif.");
                }
            }
            else
            {
                _logger.LogWarning("Failed to validate account {AccountId}: {Message}", accountId, accountsResponse.Message);
                throw new BadRequestException("Impossible de valider le compte. Veuillez réessayer.");
            }
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating account {AccountId}", accountId);
            throw new BadRequestException("Erreur lors de la validation du compte. Veuillez réessayer.");
        }

        // AC-8: Validate CategoryId exists and is active
        try
        {
            var categoriesResponse = await _tresorerieService.GetCategoriesAsync(
                "depensio",
                boutiqueId.ToString(),
                type: CategoryType.EXPENSE,
                includeInactive: false);

            if (categoriesResponse.Success && categoriesResponse.Data != null)
            {
                var categoryExists = categoriesResponse.Data.Categories.Any(c => c.Id.ToString() == categoryId && c.IsActive);
                if (!categoryExists)
                {
                    throw new BadRequestException($"La catégorie avec l'ID {categoryId} n'existe pas ou n'est pas active.");
                }
            }
            else
            {
                _logger.LogWarning("Failed to validate category {CategoryId}: {Message}", categoryId, categoriesResponse.Message);
                throw new BadRequestException("Impossible de valider la catégorie. Veuillez réessayer.");
            }
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating category {CategoryId}", categoryId);
            throw new BadRequestException("Erreur lors de la validation de la catégorie. Veuillez réessayer.");
        }
    }
}
