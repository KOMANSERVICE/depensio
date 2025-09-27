
using depensio.Shared.Models;
using depensio.Shared.Pages.Auth.Models;
using Refit;

namespace depensio.Shared.Services;

public interface IAuthUserService
{
    [Post("/getlisteuser/{boutiqueId}")]
    Task<BaseResponse<GetListeUserResponse>> GetListeUser(Guid boutiqueId);
}
