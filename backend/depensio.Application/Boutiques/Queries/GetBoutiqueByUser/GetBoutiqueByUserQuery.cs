

namespace depensio.Application.Boutiques.Queries.GetBoutiqueByUser;


public record GetBoutiqueByUserQuery()
    : IQuery<GetBoutiqueByUserResult>;
public record GetBoutiqueByUserResult(IEnumerable<BoutiqueDTO> Boutiques);
