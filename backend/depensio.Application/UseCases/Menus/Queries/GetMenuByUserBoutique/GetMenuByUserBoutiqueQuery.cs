using depensio.Application.UseCases.Menus.DTOs;

namespace depensio.Application.UseCases.Menus.Queries.GetMenuByUserBoutique;

public record GetMenuByUserBoutiqueQuery(Guid BoutiqueId)
    : IQuery<GetMenuByUserBoutiqueResult>;

public record GetMenuByUserBoutiqueResult(IEnumerable<MenuUserDTO> Menus);
