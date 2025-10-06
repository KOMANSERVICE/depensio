using depensio.Application.UseCases.Dashboard.DTOs;

namespace depensio.Application.UseCases.Dashboards.Commands.GetSalesDetailByBoutique;

public class GetSalesDetailByBoutiqueHandler(
        IDepensioDbContext _dbContext,
        IUserContextService _userContext
    )
    : ICommandHandler<GetSalesDetailByBoutiqueCommand, GetSalesDetailByBoutiqueResult>
{
    public async Task<GetSalesDetailByBoutiqueResult> Handle(GetSalesDetailByBoutiqueCommand request, CancellationToken cancellationToken)
    {
        var boutiqueId = request.BoutiqueId;
        var userId = _userContext.GetUserId();

        var sales = await _dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(boutiqueId)
                && b.UsersBoutiques.Any(ub => ub.BoutiqueId == BoutiqueId.Of(boutiqueId) && ub.UserId == userId))
            .SelectMany(b => b.Products)
            .SelectMany(product => product.SaleItems
                .Where(si => si.Sale.Date >= request.SaleRequest.StartDate && si.Sale.Date <= request.SaleRequest.EndDate)
                .Select(si => new SaleDashboardDTO
                {
                    Name = product.Name,
                    SaleDate = si.Sale.Date,
                    SalePrice = si.Price,
                    Quantity = si.Quantity,
                    TotalAmount = si.Price * si.Quantity,
                    AveragePurchasePrice = product.PurchaseItems.Any() ? product.PurchaseItems.Average(pi => pi.Price) : 0,
                    Balance = (si.Price * si.Quantity) - (product.PurchaseItems.Any() ? product.PurchaseItems.Average(pi => pi.Price) * si.Quantity : 0)
                })
            )
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

        return new GetSalesDetailByBoutiqueResult(sales);
    }
}
