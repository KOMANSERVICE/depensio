namespace depensio.Application.UseCases.Menus.DTOs;

public record MenuDTO(
    Guid Id,
    string Name
);

public record MenuUserDTO(
    Guid Id,
    string Name,
    string UrlFront,
    string Icon
);
