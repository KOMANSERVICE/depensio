using depensio.Application.UseCases.Purchases.DTOs;
using depensio.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Items = p.PurchaseItems.Select(pi => new PurchaseItemDTO(
                    pi.Id.Value,pi.ProductId.Value, pi.Price, pi.Quantity
                )).ToList()
            })
            .ToListAsync();


        return new GetPurchaseByBoutiqueResult(purchases);
    }
}
