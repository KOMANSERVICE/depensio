
using depensio.Application.UseCases.Profiles.DTO;

namespace depensio.Application.UseCases.Profiles.Queries.GetProfileByBoutique;

public class GetProfileByBoutiqueHandler(
        IDepensioDbContext _dbContext,
        IUserContextService _userContextService
    )
    : IQueryHandler<GetProfileByBoutiqueQuery, GetProfileByBoutiqueResult>
{
    public async Task<GetProfileByBoutiqueResult> Handle(GetProfileByBoutiqueQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId();
        var profiles = await _dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(request.BoutiqueId)
                && b.UsersBoutiques.Any(u => u.UserId == userId))
            .Include(b => b.Profiles)
                .ThenInclude(p => p.ProfileMenus)
                    .ThenInclude(pm => pm.Menu)
            .SelectMany(b => b.Profiles)
            .Select(p => new ProfileDTO(
                p.Id.Value,
                p.Name,
                p.IsActive,
                p.ProfileMenus.Select(pm => new ProfileMenuDTO(
                    pm.MenuId.Value,
                    pm.IsActive,
                    pm.Menu != null ? pm.Menu.Name : string.Empty
                )).ToList()
            ))
            .ToListAsync();

        return new GetProfileByBoutiqueResult(profiles);
    }
}
