

namespace depensio.Shared.Pages.Auth.Models;


public record SignUpRequest(SignUpDTO Signup);
public record SignUpResponse(bool Result);
public record SignInRequest(SignInDTO Signin);
public record SignInResponse(string Token);
public record VerifyMailRequest(VerifyMailDTO VerifyMail);
public record VerifyMailResponse(bool Result);
public record SignInDTO()
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}
public record VerifyMailDTO(string Id, string Code);
public record SignUpDTO
{
    public string Email { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Tel { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPasswords { get; set; } = string.Empty;
}
public record SignUpBoutiqueDTO
{
    public string Email { get; set; } = string.Empty;
    public Guid BoutiqueId { get; set; }
}

public record UserBoutiqueDTO
{
    public string Email { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public Guid BoutiqueId { get; set; }
    public Guid ProfileId { get; set; }
}
public record CreateUserRequest(SignUpBoutiqueDTO Signup);
public record CreateUserResponse(bool Result);

public record GetListeUserResponse(IEnumerable<UserBoutiqueDTO> ListeUsers);

public record GetUserInfosResponse(UserInfosDTO UserInfos);
public record UserInfosDTO
{
    public string Email { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Tel { get; set; } = string.Empty;
}
public record UpdateUserRequest(UserInfosDTO UserInfos);
public record UpdateUserResponse(bool Result);
public record ForgotPasswordDTO
{
    public string Email { get; set; } = string.Empty;
}
public record ForgotPasswordRequest(ForgotPasswordDTO ForgotPassword);
public record ForgotPasswordResponse(bool Result);

public record ResetPasswordDTO
{
    public string Id { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public record ResetPasswordRequest(ResetPasswordDTO ResetPassword);
public record ResetPasswordResponse(bool Result);
public record SendVerifyMailRequest(string UserId);
public record SendVerifyMailResponse(bool Result);