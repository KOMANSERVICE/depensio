using depensio.Application.UseCases.Profiles.Commands.AssignedProfileToUser;
using depensio.Application.UseCases.Profiles.DTO;

namespace depensio.Application.UseCases.Profiles.Commands.AssignedProfileToUser;

public record AssignedProfileToUserCommand(Guid BoutiqueId, AssigneProfileDTO AssigneProfile)
    : ICommand<AssignedProfileToUserResult>;

public record AssignedProfileToUserResult(
    Guid Id
);

public class AssignedProfileToUserValidator
    : AbstractValidator<AssignedProfileToUserCommand>
{
    public AssignedProfileToUserValidator()
    {
        RuleFor(x => x.BoutiqueId).NotEmpty().WithMessage("BoutiqueId is required.");
        RuleFor(x => x.AssigneProfile).NotNull().WithMessage("AssigneProfile is required.");
        RuleFor(x => x.AssigneProfile.ProfileId).NotEmpty().WithMessage("ProfileId is required.");
    }
}