
using depensio.Shared.Pages.Auth.Models;
using Refit;

namespace depensio.Shared.Services;

public interface IAuthService
{
    [Post("/signup")]
    Task<SignUpResponse> SignUp(SignUpRequest request);
    [Post("/signin")]
    Task<SignInResponse> SignIn(SignInRequest request);
    [Post("/verifymail")]
    Task<VerifyMailResponse> VerifyMail(VerifyMailRequest request);
}
