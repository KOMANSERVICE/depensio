using depensio.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace depensio.Application.Services;

public class AuthorizationService(
        IDepensioDbContext _dbContext
    )
{

    public void GetUserPermissionForBoutique(string userId, Guid boutiqueId)
    {
        var authBoutique = _dbContext.Boutiques
                  .Any(b => b.Id == BoutiqueId.Of(boutiqueId)
                              && b.UsersBoutiques.Any(ub => ub.UserId == userId));
        if (!authBoutique)
        {
            throw new UnauthorizedException("You are not authorized to add barcode to this product");
        }
    }
}
