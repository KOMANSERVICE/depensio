using depensio.Application.UseCases.Purchases.DTOs;
using depensio.Domain.Enums;
using depensio.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Purchases.Commands.UpdatePurchase;

public class UpdatePurchaseHandler(
    IDepensioDbContext _depensioDbContext,
    IGenericRepository<Purchase> _purchaseRepository,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    ILogger<UpdatePurchaseHandler> _logger
    )
    : ICommandHandler<UpdatePurchaseCommand, UpdatePurchaseResult>
{
    public async Task<UpdatePurchaseResult> Handle(
        UpdatePurchaseCommand command,
        CancellationToken cancellationToken
        )
    {
        var purchaseId = PurchaseId.Of(command.Purchase.Id);
        var boutiqueId = BoutiqueId.Of(command.Purchase.BoutiqueId);

        // AC-1: Retrieve purchase and verify it exists
        var purchase = await _depensioDbContext.Purchases
            .Include(p => p.PurchaseItems)
            .FirstOrDefaultAsync(p => p.Id == purchaseId && p.BoutiqueId == boutiqueId, cancellationToken);

        if (purchase == null)
        {
            throw new NotFoundException($"L'achat avec l'ID {command.Purchase.Id} n'existe pas.", nameof(command.Purchase.Id));
        }

        // AC-1 & AC-6: Only Draft or Rejected purchases can be modified
        var currentStatus = (PurchaseStatus)purchase.Status;
        if (currentStatus != PurchaseStatus.Draft && currentStatus != PurchaseStatus.Rejected)
        {
            throw new BadRequestException($"Impossible de modifier un achat avec le statut '{currentStatus}'. Seuls les achats en brouillon ou rejetés peuvent être modifiés.");
        }

        // Verify all products exist
        var existingProductIds = await _depensioDbContext.Products
            .Select(p => p.Id.Value)
            .ToListAsync(cancellationToken);

        var nonExistentItems = command.Purchase.Items
            .Any(item => !existingProductIds.Contains(item.ProductId));
        if (nonExistentItems)
        {
            throw new NotFoundException("Un ou plusieurs produits n'existent pas.", nameof(command.Purchase.Items));
        }

        var userId = _userContextService.GetUserId();

        // AC-2 & AC-3: Update all fields
        UpdatePurchaseFields(purchase, command.Purchase, userId);

        // AC-3: Update items (remove existing and add new ones)
        await UpdatePurchaseItems(purchase, command.Purchase.Items, cancellationToken);

        // AC-4: Recalculate TotalAmount
        purchase.TotalAmount = command.Purchase.Items.Sum(i => i.Price * i.Quantity);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} updated successfully by user {UserId}", purchase.Id.Value, userId);

        return new UpdatePurchaseResult(purchase.Id.Value);
    }

    private void UpdatePurchaseFields(Purchase purchase, PurchaseDTO purchaseDTO, string userId)
    {
        // AC-2: All fields can be modified
        purchase.Title = purchaseDTO.Title;
        purchase.Description = purchaseDTO.Description;
        purchase.SupplierName = purchaseDTO.SupplierName;
        purchase.DateAchat = purchaseDTO.DateAchat;

        // Optional payment fields
        purchase.PaymentMethod = purchaseDTO.PaymentMethod;
        purchase.AccountId = purchaseDTO.AccountId;
        purchase.CategoryId = purchaseDTO.CategoryId;

        // AC-5: UpdatedAt is handled by Entity framework auditing
        // If Status was Rejected and we're modifying, it stays as Draft
        if ((PurchaseStatus)purchase.Status == PurchaseStatus.Rejected)
        {
            // When modifying a rejected purchase, reset to Draft
            purchase.Status = (int)PurchaseStatus.Draft;
            purchase.RejectionReason = null;
        }
    }

    private async Task UpdatePurchaseItems(Purchase purchase, IEnumerable<PurchaseItemDTO> newItems, CancellationToken cancellationToken)
    {
        // AC-3: Items can be added, modified, or deleted
        // Strategy: Remove all existing items and add the new ones

        // Get existing item IDs
        var existingItemIds = purchase.PurchaseItems.Select(i => i.Id.Value).ToList();

        // Remove existing items from DbContext
        foreach (var existingItem in purchase.PurchaseItems.ToList())
        {
            _depensioDbContext.PurchaseItems.Remove(existingItem);
        }

        // Clear the collection
        purchase.PurchaseItems.Clear();

        // Add new items
        foreach (var itemDto in newItems)
        {
            var newItem = new PurchaseItem
            {
                Id = PurchaseItemId.Of(Guid.NewGuid()),
                PurchaseId = purchase.Id,
                ProductId = ProductId.Of(itemDto.ProductId),
                Price = itemDto.Price,
                Quantity = itemDto.Quantity
            };
            purchase.PurchaseItems.Add(newItem);
        }
    }
}
