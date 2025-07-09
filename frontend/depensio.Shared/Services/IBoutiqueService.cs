using depensio.Shared.Models;
using depensio.Shared.Pages.Boutique.Model;
using Refit;

namespace depensio.Shared.Services;

public interface IBoutiqueService
{
    [Post("/boutique")]
    Task<BaseResponse<CreateBoutiqueResponse>> CreateBoutique(CreateBoutiqueRequest request, CancellationToken cancellationToken = default); 
    [Get("/boutique/user")]
    Task<BaseResponse<GetBoutiqueByUserResponse>> GetBoutiqueByUser(CancellationToken cancellationToken = default);
}
