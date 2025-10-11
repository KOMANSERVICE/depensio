using depensio.Application.UserCases.Boutiques.DTOs;

namespace depensio.Application.UserCases.Boutiques.Commands.CreateBoutique;

public record CreateBoutiqueCommand(BoutiqueDTO Boutique)
    : ICommand<CreateBoutiqueResult>;

public record CreateBoutiqueResult(Guid Id);

public class CreateBoutiqueCommandValidator : AbstractValidator<CreateBoutiqueCommand>
{
    public CreateBoutiqueCommandValidator()
    {
        RuleFor(x => x.Boutique)
            .NotNull().WithMessage("La boutique est obligatoire.");
        
        RuleFor(x => x.Boutique.Name)
            .NotEmpty().WithMessage("Le nom est obligatoire.");
        
        RuleFor(x => x.Boutique.Location)
            .NotEmpty().WithMessage("L'emplacement est obligatoire.");
    }
}

