using depensio.Shared.Models;
using depensio.Shared.Pages.Magasin.Models;
using Refit;

namespace depensio.Shared.Services;

public interface IMagasinService
{
    [Get("/stocklocation/{boutiqueId}")]
    Task<BaseResponse<GetMagasinsByBoutiqueResponse>> GetMagasinsByBoutiqueAsync(Guid boutiqueId, CancellationToken cancellationToken = default);

    [Post("/stocklocation/{boutiqueId}")]
    Task<BaseResponse<CreateMagasinResponse>> CreateMagasinAsync(Guid boutiqueId, CreateMagasinRequest request, CancellationToken cancellationToken = default);
}
