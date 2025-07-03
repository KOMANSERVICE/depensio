namespace depensio.Application.Auth.DTOs;

public record SignUpDTO
{
    public string Email { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string Tel { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPasswords { get; set; } = default!;
}
