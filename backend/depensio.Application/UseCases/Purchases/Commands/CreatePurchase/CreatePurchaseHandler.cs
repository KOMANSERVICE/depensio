
using depensio.Application.UseCases.Purchases.Commands.CreatePurchase;
using depensio.Application.UseCases.Purchases.DTOs;
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

        return new Purchase
        {
            Id = purchaseId,
            Date = DateTime.UtcNow,
            Title = purchaseDTO.Title,
            Description = purchaseDTO.Description,
            SupplierName = purchaseDTO.SupplierName,
            DateAchat = purchaseDTO.DateAchat,
            PurchaseItems = purchaseDTO.Items.Select(i => new PurchaseItem
            {
                Id = PurchaseItemId.Of(Guid.NewGuid()),
                ProductId = ProductId.Of(i.ProductId),
                Price = i.Price,
                Quantity = i.Quantity,
                PurchaseId = purchaseId
            }).ToList(),
            BoutiqueId = BoutiqueId.Of(purchaseDTO.BoutiqueId),
        };
    }
}
