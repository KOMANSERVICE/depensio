namespace depensio.Application.UseCases.Auth.DTOs;

public record ResetPasswordDTO
{
    public string Id { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

