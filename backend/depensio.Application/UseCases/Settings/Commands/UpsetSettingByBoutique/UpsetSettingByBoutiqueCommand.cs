namespace depensio.Application.UserCases.Boutiques.Commands.UpsetSettingByBoutique;

public record UpsetSettingByBoutiqueCommand(SettingDTO Setting)
    : ICommand<UpsetSettingByBoutiqueResult>;

public record UpsetSettingByBoutiqueResult(Guid Id);

public class UpsetSettingByBoutiqueCommandValidator : AbstractValidator<UpsetSettingByBoutiqueCommand>
{
    public UpsetSettingByBoutiqueCommandValidator()
    {
        RuleFor(x => x.Setting)
            .NotNull().WithMessage("Le paramètre est obligatoire.");
        
        RuleFor(x => x.Setting.Key)
            .NotEmpty().WithMessage("La clé est obligatoire.");
        
        RuleFor(x => x.Setting.Value)
            .NotEmpty().WithMessage("La valeur est obligatoire.");
    }
}

