
using depensio.Application.ApiExterne.Tresoreries;
using depensio.Application.UseCases.Purchases.Commands.CreatePurchase;
using depensio.Application.UseCases.Purchases.DTOs;
using depensio.Domain.Enums;
using depensio.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Purchases.Commands.CreatePurchase;

public class CreatePurchaseHandler(
    IDepensioDbContext _depensioRepository,
    IGenericRepository<Purchase> _purchaseRepository,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    ITresorerieService _tresorerieService,
    ILogger<CreatePurchaseHandler> _logger
    )
    : ICommandHandler<CreatePurchaseCommand, CreatePurchaseResult>
{
    public async Task<CreatePurchaseResult> Handle(
        CreatePurchaseCommand command,
        CancellationToken cancellationToken
        )
    {
        var boutiqueExite = await _depensioRepository.Boutiques
            .AnyAsync(b => b.Id == BoutiqueId.Of(command.Purchase.BoutiqueId), cancellationToken);
        if (!boutiqueExite)
        {
            throw new NotFoundException($"Boutique with ID {command.Purchase.BoutiqueId} does not exist.", nameof(command.Purchase.BoutiqueId));
        }

        // Récupère tous les IDs des produits dans la base
        var existingProductIds = await _depensioRepository.Products
            .Select(p => p.Id.Value)
            .ToListAsync(cancellationToken);

        // Filtre les items du purchaseDTO qui ne sont pas encore dans la base
        var nonExistentItems = command.Purchase.Items
            .Any(item => !existingProductIds.Contains(item.ProductId));
        if (nonExistentItems)
        {
            throw new NotFoundException($"Any product does not exist.", nameof(command.Purchase.BoutiqueId));
        }

        var email = _userContextService.GetEmail();
        var purchase = CreateNewPurchase(command.Purchase, email);

        await _purchaseRepository.AddDataAsync(purchase, cancellationToken);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        // AC-3: If PaymentMethod + AccountId provided AND status is Approved → Call Trésorerie to create CashFlow
        // AC-4: If payment info absent → No Trésorerie call (current behavior)
        if (purchase.Status == (int)PurchaseStatus.Approved &&
            !string.IsNullOrEmpty(purchase.PaymentMethod) &&
            purchase.AccountId.HasValue &&
            !string.IsNullOrEmpty(purchase.CategoryId))
        {
            await CreateCashFlowFromPurchaseAsync(purchase, command.Purchase.BoutiqueId);
        }

        return new CreatePurchaseResult(purchase.Id.Value);
    }

    private Purchase CreateNewPurchase(PurchaseDTO purchaseDTO, string email)
    {
        var purchaseId = PurchaseId.Of(Guid.NewGuid());
        var now = DateTime.UtcNow;

        // AC-1: If status not specified in request → Status = Approved (3) (retrocompatibility)
        var isDraft = string.Equals(purchaseDTO.Status, "draft", StringComparison.OrdinalIgnoreCase);
        var status = isDraft ? PurchaseStatus.Draft : PurchaseStatus.Approved;

        // Calculate total amount from items
        var totalAmount = purchaseDTO.Items.Sum(i => i.Price * i.Quantity);

        // AC-6: Create initial status history entry (FromStatus = null, ToStatus = status)
        var statusHistory = new PurchaseStatusHistory
        {
            Id = PurchaseStatusHistoryId.Of(Guid.NewGuid()),
            PurchaseId = purchaseId,
            FromStatus = null, // AC-6: FromStatus = null for initial creation
            ToStatus = (int)status,
            Comment = isDraft ? "Achat créé en brouillon" : "Achat créé et approuvé"
        };

        return new Purchase
        {
            Id = purchaseId,
            Date = now,
            Title = purchaseDTO.Title,
            Description = purchaseDTO.Description,
            SupplierName = purchaseDTO.SupplierName,
            DateAchat = purchaseDTO.DateAchat,
            Status = (int)status,
            TotalAmount = totalAmount,
            // AC-2: Optional fields (PaymentMethod, AccountId, CategoryId)
            PaymentMethod = purchaseDTO.PaymentMethod,
            AccountId = purchaseDTO.AccountId,
            CategoryId = purchaseDTO.CategoryId,
            // CashFlowId will be set after Trésorerie call if applicable
            CashFlowId = null,
            // AC-5: ApprovedAt = CreatedAt, ApprovedBy = CreatedBy (when status is Approved)
            ApprovedAt = isDraft ? null : now,
            ApprovedBy = isDraft ? null : email,
            PurchaseItems = purchaseDTO.Items.Select(i => new PurchaseItem
            {
                Id = PurchaseItemId.Of(Guid.NewGuid()),
                ProductId = ProductId.Of(i.ProductId),
                Price = i.Price,
                Quantity = i.Quantity,
                PurchaseId = purchaseId
            }).ToList(),
            BoutiqueId = BoutiqueId.Of(purchaseDTO.BoutiqueId),
            // AC-6: Create initial status history entry
            StatusHistory = new List<PurchaseStatusHistory> { statusHistory }
        };
    }

    /// <summary>
    /// AC-3: Call Trésorerie service to create CashFlow from approved purchase
    /// </summary>
    private async Task CreateCashFlowFromPurchaseAsync(Purchase purchase, Guid boutiqueId)
    {
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
                boutiqueId.ToString(),
                request
            );

            if (result.Success && result.Data != null)
            {
                // Update purchase with CashFlowId
                purchase.CashFlowId = result.Data.CashFlow.Id;
                await _unitOfWork.SaveChangesDataAsync(default);
                _logger.LogInformation("CashFlow {CashFlowId} created for Purchase {PurchaseId}", result.Data.CashFlow.Id, purchase.Id.Value);
            }
            else
            {
                _logger.LogWarning("Failed to create CashFlow for Purchase {PurchaseId}: {Message}", purchase.Id.Value, result.Message);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the purchase creation
            _logger.LogError(ex, "Error creating CashFlow for Purchase {PurchaseId}", purchase.Id.Value);
        }
    }
}
