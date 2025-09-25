namespace depensio.Application.UseCases.Auth.DTOs;

public record ForgotPasswordDTO
{
    public string Email { get; set; } = string.Empty;
}

