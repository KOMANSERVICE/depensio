using depensio.Application.UseCases.Profiles.DTO;

namespace depensio.Application.UseCases.Profiles.Queries.GetProfileByBoutique;

public record GetProfileByBoutiqueQuery(Guid BoutiqueId) 
    : IQuery<GetProfileByBoutiqueResult>;


public record GetProfileByBoutiqueResult(IEnumerable<ProfileDTO> Profiles);