namespace depensio.Application.UseCases.Profiles.DTO;

public record ProfileDTO(
    Guid Id,
    string Name, bool IsActive,
    List<ProfileMenuDTO> MenuIds
);

public record ProfileMenuDTO(
    Guid MenuId,
    bool IsActive,
    string Name
);

public record AssigneProfileDTO(
    Guid ProfileId
);

