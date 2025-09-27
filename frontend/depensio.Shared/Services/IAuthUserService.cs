
using depensio.Shared.Models;
using depensio.Shared.Pages.Auth.Models;
using Refit;

namespace depensio.Shared.Services;

public interface IAuthUserService
{
    [Get("/getlisteuser/{boutiqueId}")]
    Task<BaseResponse<GetListeUserResponse>> GetListeUser(Guid boutiqueId);
    [Post("/createuser")]
    Task<BaseResponse<CreateUserResponse>> CreateUserAsync(CreateUserRequest request);
    [Get("/auth/getuserinfos")]
    Task<BaseResponse<GetUserInfosResponse>> GetUserInfosAsync();
}
