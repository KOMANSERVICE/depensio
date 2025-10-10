namespace depensio.Application.UseCases.Boutiques.Queries.GetOneBoutiqueByUser;

public record GetOneBoutiqueByUserQuery(Guid BoutiqueId)
    : IQuery<GetOneBoutiqueByUserResult>;
public record GetOneBoutiqueByUserResult(BoutiqueDTO Boutique);