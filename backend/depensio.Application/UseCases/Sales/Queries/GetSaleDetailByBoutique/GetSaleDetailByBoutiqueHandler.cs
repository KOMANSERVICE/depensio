using depensio.Application.UseCases.Sales.DTOs;
using depensio.Domain.Enums;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Sales.Queries.GetSaleDetailByBoutique;

public class GetSaleDetailByBoutiqueHandler(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetSaleDetailByBoutiqueQuery, GetSaleDetailByBoutiqueResult>
{
    public async Task<GetSaleDetailByBoutiqueResult> Handle(GetSaleDetailByBoutiqueQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();


        var salesDetail = await dbContext.Boutiques
                         .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
                                     && b.UsersBoutiques.Any(ub => ub.UserId == userId))
                         .Include(b => b.Sales)
                             .ThenInclude(s => s.SaleItems)
                         .SelectMany(b => b.Sales.Where(s => s.Status != SaleStatus.Cancelled).SelectMany(s => s.SaleItems))
                         .Join(dbContext.Products,
                             saleItem => saleItem.ProductId,
                             product => product.Id,
                             (saleItem, product) => new { saleItem, product })
                         .Select(g => new SaleDetailDTO(
                             g.product.Id.Value,
                             g.product.Name,
                             g.saleItem.Quantity,
                             g.saleItem.Price,
                             g.saleItem.Quantity * g.saleItem.Price,
                             DateTime.Now                             
                         )).ToListAsync();


        return new GetSaleDetailByBoutiqueResult(salesDetail);
    }
}
