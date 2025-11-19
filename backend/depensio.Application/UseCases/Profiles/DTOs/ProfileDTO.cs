namespace depensio.Application.UseCases.Profiles.DTO;

public record ProfileDTO(
    Guid Id,
    string Name, bool IsActive,
    List<ProfileMenuDTO> MenuIds
);

public record ProfileMenuDTO(
    string Reference,
    bool IsActive,
    string Name
);

public record AssigneProfileDTO(
    string Email,
    Guid ProfileId
);

