namespace depensio.Application.UseCases.Purchases.Commands.SubmitPurchase;

/// <summary>
/// Command to submit a draft purchase for approval (Draft -> PendingApproval)
/// </summary>
public record SubmitPurchaseCommand(Guid PurchaseId, Guid BoutiqueId)
    : ICommand<SubmitPurchaseResult>;

public record SubmitPurchaseResult(Guid Id, string Status);

public class SubmitPurchaseCommandValidator : AbstractValidator<SubmitPurchaseCommand>
{
    public SubmitPurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty().WithMessage("L'identifiant de l'achat est obligatoire.");

        RuleFor(x => x.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
    }
}
