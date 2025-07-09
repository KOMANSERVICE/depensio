using depensio.Application.UseCases.Products.DTOs;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Products.Queries.GetProductByUser;


public class GetProductByUserHandler(
    IDepensioDbContext dbContext,
    IUserContextService _userContextService)
    : IQueryHandler<GetProductByUserQuery, GetProductByUserResult>
{
    public async Task<GetProductByUserResult> Handle(GetProductByUserQuery request, CancellationToken cancellationToken)
    {

        var userId = _userContextService.GetUserId();

        //var userProduct = await dbContext.Boutiques
        //    .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId) && b.UsersBoutiques.Any(ub => ub.UserId == userId))
        //    .Select(b => new ProductDTO(
        //        b.Products.,
        //        b.Name,
        //        b.Location
        //    ))
        //    .ToListAsync();

        var products = await dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
                        && b.UsersBoutiques.Any(ub => ub.UserId == userId))
            .Include(b => b.Products)
            .SelectMany(b => b.Products)
            .Select(p => new ProductDTO(
                p.Id.Value,
                p.BoutiqueId.Value,
                p.Name,
                p.Barcode,
                p.CostPrice,
                p.Price,
                p.Stock
            ))
            .ToListAsync();


        return new GetProductByUserResult(products);
    }
}
