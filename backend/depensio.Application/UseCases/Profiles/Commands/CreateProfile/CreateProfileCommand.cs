using depensio.Application.UseCases.Profiles.DTO;

namespace depensio.Application.UseCases.Profiles.Commands.CreateProfile;

public record CreateProfileCommand(Guid BoutiqueId, ProfileDTO Profile)
    : ICommand<CreateProfileResult>;

public record CreateProfileResult(Guid Id);

public class CreateProfileCommandValidators : AbstractValidator<CreateProfileCommand>
{
    public CreateProfileCommandValidators()
    {
        RuleFor(x => x.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
        
        RuleFor(x => x.Profile.Name)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères.");
        
        RuleFor(x => x.Profile.MenuIds)
            .NotEmpty().WithMessage("Les lignes de menu sont obligatoires.");
    }
}