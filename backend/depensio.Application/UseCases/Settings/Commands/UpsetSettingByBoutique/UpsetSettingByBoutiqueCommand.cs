
namespace depensio.Application.UserCases.Boutiques.Commands.UpsetSettingByBoutique;

public record UpsetSettingByBoutiqueCommand(SettingDTO Setting)
    : ICommand<UpsetSettingByBoutiqueResult>;

public record UpsetSettingByBoutiqueResult(Guid Id);

public class UpsetSettingByBoutiqueCommandValidator : AbstractValidator<UpsetSettingByBoutiqueCommand>
{
    public UpsetSettingByBoutiqueCommandValidator()
    {
        RuleFor(x => x.Setting).NotNull().WithMessage("Setting is required");
        RuleFor(x => x.Setting.Key).NotEmpty().WithMessage("Key is required");
        RuleFor(x => x.Setting.Value).NotEmpty().WithMessage("Value is required");
    }
}

