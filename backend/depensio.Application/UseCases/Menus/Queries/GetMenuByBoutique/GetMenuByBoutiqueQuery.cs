using depensio.Application.UseCases.Menus.DTOs;

namespace depensio.Application.UseCases.Menus.Queries.GetMenuByBoutique;

public record GetMenuByBoutiqueQuery(Guid BoutiqueId) 
    : IQuery<GetMenuByBoutiqueResult>;
public record GetMenuByBoutiqueResult(IEnumerable<MenuDTO> Menus);
