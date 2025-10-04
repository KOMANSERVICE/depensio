namespace depensio.Shared.Pages.Profiles.Models;

public record ProfileMenu{
    public Guid MenuId { get; set; } = Guid.Empty;
    public bool IsActive { get; set; }
    public string Name { get; set; } = string.Empty;
}

public record Profile{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<ProfileMenu> MenuIds { get; set; } = new();
}
public record AssigneProfile{
    public string Email { get; set; } = string.Empty;
    public Guid ProfileId { get; set; } = Guid.Empty;
}

public record Menu(
    Guid Id,
    string Name
);

public record GetMenuByBoutiqueResponse(IEnumerable<Menu> Menus);
public record GetProfileByBoutiqueResponse(IEnumerable<Profile> Profiles);


public record CreateProfileRequest(Profile Profile);
public record CreateProfileResponse(Guid Id);
public record AssignedProfileToUserRequest(AssigneProfile AssignedProfile);
public record AssignedProfileToUserResponse(Guid Id);
