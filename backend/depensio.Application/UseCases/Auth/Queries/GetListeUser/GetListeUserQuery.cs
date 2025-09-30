namespace depensio.Application.UseCases.Auth.Queries.ListeUser;


public record GetListeUserQuery(Guid BoutiqueId)
    : IQuery<GetListeUserResult>;

public record GetListeUserResult(IEnumerable<UserBoutiqueDTO> ListeUsers);
