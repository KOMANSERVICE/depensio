using depensio.Application.UseCases.Profiles.DTO;

namespace depensio.Application.UseCases.Profiles.Commands.CreateProfile;

public record CreateProfileCommand(Guid BoutiqueId, ProfileDTO Profile)
    :ICommand<CreateProfileResult>;

public record CreateProfileResult(Guid Id);

public class CreateProfileCommandValidators : AbstractValidator<CreateProfileCommand>
{
    public CreateProfileCommandValidators()
    {
        RuleFor(x => x.BoutiqueId).NotEmpty().WithMessage("BoutiqueId est obligatoire.");
        RuleFor(x => x.Profile.Name).NotEmpty().WithMessage("Name est obligatoire.")
                                       .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
        RuleFor(x => x.Profile.MenuIds).NotEmpty().WithMessage("Ligne de menu est obligatoire.");
    }

}