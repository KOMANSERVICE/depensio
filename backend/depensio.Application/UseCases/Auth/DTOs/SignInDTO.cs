namespace depensio.Application.UserCases.Auth.DTOs;

public record SignInDTO(string Email, string Password, bool RememberMe);
