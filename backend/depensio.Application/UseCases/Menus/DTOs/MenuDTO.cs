namespace depensio.Application.UseCases.Menus.DTOs;

public record MenuDTO(
    Guid Id,
    string Name
);

public record MenuUserDTO{
    public Guid Id { get; set; } = Guid.Empty;  
    public string Name { get; set; } = string.Empty;
    public string UrlFront { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
