namespace depensio.Application.UserCases.Auth.DTOs;

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
