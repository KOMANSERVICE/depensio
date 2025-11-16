using depensio.Application.ApiExterne.Menus;
using depensio.Application.Interfaces;
using depensio.Application.UseCases.Menus.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace depensio.Application.Services;

public class MenuService(
        IDepensioDbContext _dbContext,
        IMenuService menuServiceApi
    ) 

{

    public async Task<List<MenuUserDTO>> GetMenuByUserBoutiqueAsync(string userId, Guid boutiqueId)
    {
        //var isOwner = await _dbContext.Boutiques
        //    .AnyAsync(b => b.Id == BoutiqueId.Of(boutiqueId)
        //    && b.OwnerId == userId
        //    && b.UsersBoutiques.Any(ub => ub.UserId == userId));

        //List<MenuUserDTO> menus;

        //if (isOwner)
        //{
        //    //TODO: (A revoir, reccuperer les menus du plan) deja correct
        //    // juste mettre un filtre sur chaque API pour s'assurer que le menu fait partir de l`'abom=nement
        //    // Le propritaire doit voir tous les menus, le menu ne fesant pas partir de sont plan seron bloquer
        //    // Propriétaire : tous les menus
        //    menus = await _dbContext.Menus
        //        .Where(m => !string.IsNullOrEmpty(m.Name))
        //        .OrderBy(m => m.Order)
        //        .Select(m => new MenuUserDTO
        //        {
        //            Id = m.Id.Value,
        //            Name = m.Name,
        //            UrlFront = m.UrlFront,
        //            Icon = m.Icon
        //        })
        //        .ToListAsync();
        //}
        //else
        //{
        //    var userboutique = await _dbContext.UsersBoutiques
        //    .FirstOrDefaultAsync(ub => ub.BoutiqueId == BoutiqueId.Of(boutiqueId) && ub.UserId == userId);

        //    // Utilisateur classique : menus du profil
        //    menus = await _dbContext.Profiles
        //    .Where(p => p.Id == userboutique.ProfileId && p.IsActive)
        //    .SelectMany(p => p.ProfileMenus.Where(pm => pm.IsActive && pm.Menu != null).OrderBy(m => m.Menu.Order))
        //    .Select(pm => new MenuUserDTO
        //    {
        //        Id = pm.Menu.Id.Value,
        //        Name = pm.Menu.Name,
        //        UrlFront = pm.Menu.UrlFront,
        //        Icon = pm.Menu.Icon
        //    })
        //    .ToListAsync();
        //}

        var result = await menuServiceApi.GetAllActifMenuAsync("depensio");
        if (!result.Success)
        {
            throw new BadRequestException(result.Message);
        }

        var menus = result.Data.Menus;

        return menus;
    }

    public async Task<MenuUserDTO> GetOneMenuByUserBoutiqueAsync(string userId, Guid boutiqueId, string currentPath)
    {
        var isOwner = await _dbContext.Boutiques
            .AnyAsync(b => b.Id == BoutiqueId.Of(boutiqueId)
            && b.OwnerId == userId
            && b.UsersBoutiques.Any(ub => ub.UserId == userId));

        MenuUserDTO menu;

        if (isOwner)
        {
            //TODO: A revoir, reccuperer les menus du plan
            // Propriétaire : tous les menus
            menu = await _dbContext.Menus
                .Where(m => !string.IsNullOrEmpty(m.Name) && currentPath.StartsWith(m.UrlFront, StringComparison.OrdinalIgnoreCase))
                .OrderBy(m => m.Order)
                .Select(m => new MenuUserDTO{
                    Id=m.Id.Value,
                    Name=m.Name,
                    UrlFront = m.UrlFront,
                    Icon = m.Icon
                })
                .FirstOrDefaultAsync() ?? new MenuUserDTO();
        }
        else
        {
            var userboutique = await _dbContext.UsersBoutiques
            .FirstOrDefaultAsync(ub => ub.BoutiqueId == BoutiqueId.Of(boutiqueId) && ub.UserId == userId);

            // Utilisateur classique : menus du profil
            menu = await _dbContext.Profiles
            .Where(p => p.Id == userboutique.ProfileId && p.IsActive)
            .SelectMany(p => p.ProfileMenus.Where(pm => pm.IsActive && pm.Menu != null && !string.IsNullOrEmpty(pm.Menu.Name) && currentPath.StartsWith(pm.Menu.UrlFront, StringComparison.OrdinalIgnoreCase)).OrderBy(m => m.Menu.Order))
            .Select(pm => new MenuUserDTO
            {
                Id = pm.Menu.Id.Value,
                Name = pm.Menu.Name,
                UrlFront = pm.Menu.UrlFront,
                Icon = pm.Menu.Icon
            }).FirstOrDefaultAsync() ?? new MenuUserDTO();
        }

        return menu;
    }
}
