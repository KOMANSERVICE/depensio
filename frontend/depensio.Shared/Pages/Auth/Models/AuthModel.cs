

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
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public Guid BoutiqueId { get; set; }
}
public record CreateUserRequest(SignUpBoutiqueDTO Signup);
public record CreateUserResponse(bool Result);