using depensio.Shared.Models;
using depensio.Shared.Pages.Boutique.Model;
using Refit;

namespace depensio.Shared.Services;

public interface IBoutiqueSettingService
{
    [Post("/setting")]
    Task<BaseResponse<CreateBoutiqueResponse>> UpsetSettingByBoutique(CreateBoutiqueRequest request, CancellationToken cancellationToken = default); 
    [Get("/setting/{boutiqueId}/{key}")]
    Task<BaseResponse<GetSettingByBoutiqueResponse>> GetSettingByBoutique(Guid boutiqueId, string key,CancellationToken cancellationToken = default);
}
