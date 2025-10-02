using depensio.Application.UseCases.Sales.DTOs;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Sales.Queries.GetSaleSummaryByBoutique;

public class GetSaleSummaryByBoutiqueHandler(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetSaleSummaryByBoutiqueQuery, GetSaleSummaryByBoutiqueResult>
{
    public async Task<GetSaleSummaryByBoutiqueResult> Handle(GetSaleSummaryByBoutiqueQuery request, CancellationToken cancellationToken)
    {
        var rng = new Random();
        var userId = _userContextService.GetUserId();

        var salesSummary = await dbContext.Boutiques
     .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
                 && b.UsersBoutiques.Any(ub => ub.UserId == userId))
     .Include(b => b.Sales)
         .ThenInclude(s => s.SaleItems)
     .SelectMany(b => b.Sales.SelectMany(s => s.SaleItems))
     .Join(dbContext.Products,
         saleItem => saleItem.ProductId,
         product => product.Id,
         (saleItem, product) => new { saleItem, product })
     .GroupBy(x => new { x.product.Id, x.product.Name })
     .Select(g => new SaleSummaryDTO(
         g.Key.Id.Value,
         g.Key.Name,
         g.Sum(x => x.saleItem.Quantity),
         g.Sum(x => x.saleItem.Quantity * x.saleItem.Price),
         $"#{rng.Next(0x1000000):X6}"
     ))
     .ToListAsync();



        return new GetSaleSummaryByBoutiqueResult(salesSummary.OrderByDescending(s => s.TotalRevenue));
    }
}
