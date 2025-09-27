namespace depensio.Application.UseCases.Auth.DTOs;

public record UserInfosDTO
{
    public string Email { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Tel { get; set; } = string.Empty;
}


