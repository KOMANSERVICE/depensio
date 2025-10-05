using depensio.Application.UseCases.Menus.DTOs;

namespace depensio.Application.UseCases.Menus.Queries.GetOneMenuByUserBoutique;

public record GetOneMenuByUserBoutiqueQuery(Guid BoutiqueId, string CurrentPath)
    : IQuery<GetOneMenuByUserBoutiqueResult>;

public record GetOneMenuByUserBoutiqueResult(MenuUserDTO Menu);
