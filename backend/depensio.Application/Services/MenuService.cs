using depensio.Application.Interfaces;
using depensio.Application.UseCases.Menus.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace depensio.Application.Services;

public class MenuService(
        IDepensioDbContext _dbContext
    ) 

{

    public async Task<List<MenuUserDTO>> GetMenuByUserBoutiqueAsync(string userId, Guid boutiqueId)
    {
        var isOwner = await _dbContext.Boutiques
            .AnyAsync(b => b.Id == BoutiqueId.Of(boutiqueId)
            && b.OwnerId == userId
            && b.UsersBoutiques.Any(ub => ub.UserId == userId));

        List<MenuUserDTO> menus;

        if (isOwner)
        {
            //TODO: A revoir, reccuperer les menus du plan
            // Propriétaire : tous les menus
            menus = await _dbContext.Menus
                .Where(m => !string.IsNullOrEmpty(m.Name))
                .OrderBy(m => m.Order)
                .Select(m => new MenuUserDTO(
                    m.Id.Value,
                    m.Name,
                    m.UrlFront,
                    m.Icon
                ))
                .ToListAsync();
        }
        else
        {
            var userboutique = await _dbContext.UsersBoutiques
            .FirstOrDefaultAsync(ub => ub.BoutiqueId == BoutiqueId.Of(boutiqueId) && ub.UserId == userId);

            // Utilisateur classique : menus du profil
            menus = await _dbContext.Profiles
            .Where(p => p.Id == userboutique.ProfileId && p.IsActive)
            .SelectMany(p => p.ProfileMenus.Where(pm => pm.IsActive && pm.Menu != null).OrderBy(m => m.Menu.Order))
            .Select(pm => new MenuUserDTO(
                pm.Menu.Id.Value,
                pm.Menu.Name,
                pm.Menu.UrlFront,
                pm.Menu.Icon
            ))
            .ToListAsync();
        }

        return menus;
    }
}
