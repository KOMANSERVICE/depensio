using depensio.Application.UserCases.Boutiques.DTOs;

namespace depensio.Application.UserCases.Boutiques.Queries.GetBoutiqueByUser;


public record GetBoutiqueByUserQuery()
    : IQuery<GetBoutiqueByUserResult>;
public record GetBoutiqueByUserResult(IEnumerable<BoutiqueDTO> Boutiques);
