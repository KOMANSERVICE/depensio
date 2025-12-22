using depensio.Application.UseCases.Purchases.DTOs;
using depensio.Domain.Enums;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Purchases.Queries.GetPurchaseByBoutique;
public class GetPurchaseByBoutiqueHandler(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetPurchaseByBoutiqueQuery, GetPurchaseByBoutiqueResult>
{
    public async Task<GetPurchaseByBoutiqueResult> Handle(GetPurchaseByBoutiqueQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();

        var purchases = await dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
                        && b.UsersBoutiques.Any(ub => ub.UserId == userId))
            .Include(b => b.Purchases)
            .ThenInclude(p => p.PurchaseItems)
            .SelectMany(b => b.Purchases)
            .Select(p => new PurchaseDTO
            {
                Id = p.Id.Value,
                BoutiqueId = p.BoutiqueId.Value,
                SupplierName = p.SupplierName,
                Title = p.Title,
                Description = p.Description,
                DateAchat = p.DateAchat,
                // Map int status back to string for frontend
                Status = p.Status == (int)PurchaseStatus.Draft ? "draft" : null,
                PaymentMethod = p.PaymentMethod,
                AccountId = p.AccountId,
                CategoryId = p.CategoryId,
                Items = p.PurchaseItems.Select(pi => new PurchaseItemDTO(
                    pi.Id.Value,pi.ProductId.Value, pi.Price, pi.Quantity
                )).ToList()
            })
            .OrderByDescending(p => p.DateAchat)
            .ToListAsync();


        return new GetPurchaseByBoutiqueResult(purchases);
    }
}
