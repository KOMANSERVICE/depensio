using depensio.Application.Boutiques.DTOs;

namespace depensio.Application.Boutiques.Commands.CreateBoutique;

public record CreateBoutiqueCommand(BoutiqueDTO Boutique)
    : ICommand<CreateBoutiqueResult>;

public record CreateBoutiqueResult(Guid Id);

public class CreateBoutiqueCommandValidator : AbstractValidator<CreateBoutiqueCommand>
{
    public CreateBoutiqueCommandValidator()
    {
        RuleFor(x => x.Boutique).NotNull().WithMessage("Boutique is required");
        RuleFor(x => x.Boutique.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Boutique.Location).NotEmpty().WithMessage("Location is required");
    }
}

