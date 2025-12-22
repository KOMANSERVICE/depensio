
using depensio.Application.UseCases.Purchases.Commands.CreatePurchase;
using depensio.Application.UseCases.Purchases.DTOs;
using depensio.Domain.Enums;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Purchases.Commands.CreatePurchase;

public class CreatePurchaseHandler(
    IDepensioDbContext _depensioRepository,
    IGenericRepository<Purchase> _purchaseRepository,
    IUnitOfWork _unitOfWork
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

        var purchase = CreateNewPurchase(command.Purchase);

        await _purchaseRepository.AddDataAsync(purchase, cancellationToken);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new CreatePurchaseResult(purchase.Id.Value);
    }

    private Purchase CreateNewPurchase(PurchaseDTO purchaseDTO)
    {
        var purchaseId = PurchaseId.Of(Guid.NewGuid());

        // Determine status: if "draft" is specified, use Draft; otherwise use Approved for backward compatibility
        var isDraft = string.Equals(purchaseDTO.Status, "draft", StringComparison.OrdinalIgnoreCase);
        var status = isDraft ? PurchaseStatus.Draft : PurchaseStatus.Approved;

        // Calculate total amount from items
        var totalAmount = purchaseDTO.Items.Sum(i => i.Price * i.Quantity);

        // Create initial status history entry (AC-6)
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
            Date = DateTime.UtcNow,
            Title = purchaseDTO.Title,
            Description = purchaseDTO.Description,
            SupplierName = purchaseDTO.SupplierName,
            DateAchat = purchaseDTO.DateAchat,
            Status = (int)status,
            TotalAmount = totalAmount, // AC-7: TotalAmount is calculated and stored
            // AC-2: Optional fields (PaymentMethodId, AccountId, CategoryId)
            PaymentMethodId = purchaseDTO.PaymentMethodId,
            AccountId = purchaseDTO.AccountId,
            CategoryId = purchaseDTO.ExpenseCategoryId,
            // AC-3 & AC-4: No call to Trésorerie service, CashFlowId remains null for draft
            CashFlowId = null,
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
}
