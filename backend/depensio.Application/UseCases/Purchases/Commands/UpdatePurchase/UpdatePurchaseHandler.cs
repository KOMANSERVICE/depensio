using depensio.Application.UseCases.Purchases.DTOs;
using depensio.Domain.Enums;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace depensio.Application.UseCases.Purchases.Commands.UpdatePurchase;

public class UpdatePurchaseHandler(
    IDepensioDbContext _depensioDbContext,
    IGenericRepository<Purchase> _purchaseRepository,
    IGenericRepository<PurchaseItem> _purchaseItemRepository,
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

        var email = _userContextService.GetEmail();

        // AC-2 & AC-3: Update all fields
        await UpdatePurchaseFields(purchase, command.Purchase, email, cancellationToken);

        // AC-4: Recalculate TotalAmount
        purchase.TotalAmount = command.Purchase.Items.Sum(i => i.Price * i.Quantity);

        // Mark purchase as modified BEFORE updating items to avoid marking new items as Modified
        _purchaseRepository.UpdateData(purchase);

        // AC-3: Update items (remove existing and add new ones)
        // This must be done AFTER UpdateData to preserve correct entity states (Deleted/Added)
        //await UpdatePurchaseItems(purchase, command.Purchase.Items, cancellationToken);

        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Purchase {PurchaseId} updated successfully by user {email}", purchase.Id.Value, email);

        return new UpdatePurchaseResult(purchase.Id.Value);
    }

    private async Task UpdatePurchaseFields(Purchase purchase, PurchaseDTO purchaseDTO, string email, CancellationToken cancellationToken)
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

        var _purchaseItems = new List<PurchaseItem>();
        foreach (var itemDto in purchaseDTO.Items)
        {
            var newItem = new PurchaseItem
            {
                Id = PurchaseItemId.Of(Guid.NewGuid()),
                PurchaseId = purchase.Id,
                ProductId = ProductId.Of(itemDto.ProductId),
                Price = itemDto.Price,
                Quantity = itemDto.Quantity
            };

            _purchaseItems.Add(newItem);

        }

        // AC-5: UpdatedAt is handled by Entity framework auditing
        // If Status was Rejected and we're modifying, it stays as Draft
        if ((PurchaseStatus)purchase.Status == PurchaseStatus.Rejected)
        {
            // When modifying a rejected purchase, reset to Draft
            purchase.Status = (int)PurchaseStatus.Draft;
            purchase.RejectionReason = null;
        }


        await _purchaseItemRepository.DeleteRangeDataAsync(purchase.PurchaseItems);
        await _purchaseItemRepository.AddRangeDataAsync(_purchaseItems, cancellationToken);
    }

    private async Task UpdatePurchaseItems(Purchase purchase, IEnumerable<PurchaseItemDTO> newItems, CancellationToken cancellationToken)
    {
        // AC-3: Items can be added, modified, or deleted
        // Strategy: Remove all existing items and add the new ones

        // Remove existing items from DbContext
        await _purchaseItemRepository.DeleteRangeDataAsync(purchase.PurchaseItems);

        // Clear the collection
        purchase.PurchaseItems.Clear();

        var purchaseItems = new List<PurchaseItem>();
        // Add new items directly via DbContext to ensure proper tracking
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

            purchaseItems.Add(newItem);


        }

        await _purchaseItemRepository.AddRangeDataAsync(purchaseItems, cancellationToken);
    }
}
