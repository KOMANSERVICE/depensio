
using depensio.Shared.Models;
using depensio.Shared.Pages.Auth.Models;
using Refit;

namespace depensio.Shared.Services;

public interface IAuthHttpService
{
    [Post("/signup")]
    Task<SignUpResponse> SignUp(SignUpRequest request);
    [Post("/signin")]
    Task<SignInResponse> SignIn(SignInRequest request);
    [Post("/verifymail")]
    Task<VerifyMailResponse> VerifyMail(VerifyMailRequest request);
    [Post("/forgotpassword")]
    Task<BaseResponse<ForgotPasswordResponse>>  ForgotPasswordAsync(ForgotPasswordRequest request);
    [Post("/resetpassword")]
    Task<BaseResponse<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest request);
    
}
