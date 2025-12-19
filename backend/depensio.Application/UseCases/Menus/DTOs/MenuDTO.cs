namespace depensio.Application.UseCases.Menus.DTOs;

public record MenuDTO(
    string Reference,
    string Name
);

public record MenuUserDTO
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string UrlFront { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Group { get; set; }
    public int SortOrder { get; set; }
}
