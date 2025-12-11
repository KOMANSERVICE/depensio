using depensio.Shared.Models;
using depensio.Shared.Pages.Magasin.Models;
using Refit;

namespace depensio.Shared.Services;

public interface IMagasinService
{
    [Get("/stocklocation/{boutiqueId}")]
    Task<BaseResponse<GetMagasinsByBoutiqueResponse>> GetMagasinsByBoutiqueAsync(Guid boutiqueId, CancellationToken cancellationToken = default);

    [Get("/stocklocation/{boutiqueId}/{id}")]
    Task<BaseResponse<GetMagasinByIdResponse>> GetMagasinByIdAsync(Guid boutiqueId, Guid id, CancellationToken cancellationToken = default);

    [Post("/stocklocation/{boutiqueId}")]
    Task<BaseResponse<CreateMagasinResponse>> CreateMagasinAsync(Guid boutiqueId, CreateMagasinRequest request, CancellationToken cancellationToken = default);

    [Patch("/stocklocation/{boutiqueId}/{stockLocationId}")]
    Task<BaseResponse<UpdateMagasinResponse>> UpdateMagasinAsync(Guid boutiqueId, Guid stockLocationId, UpdateMagasinRequest request, CancellationToken cancellationToken = default);
}
