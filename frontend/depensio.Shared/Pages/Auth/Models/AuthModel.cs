

namespace depensio.Shared.Pages.Auth.Models;


public record SignUpRequest(SignUpDTO Signup);
public record SignUpResponse(bool Result);
public record SignInRequest(SignInDTO SignIn);
public record SignInResponse(string Token);
public record VerifyMailRequest(VerifyMailDTO VerifyMail);
public record VerifyMailResponse(bool Result);
public record SignInDTO(string Email, string Password);
public record VerifyMailDTO(string Id, string Code);
public record SignUpDTO
{
    public string Email { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string Tel { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPasswords { get; set; } = default!;
}