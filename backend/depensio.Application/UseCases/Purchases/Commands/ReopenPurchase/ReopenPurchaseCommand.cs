namespace depensio.Application.UseCases.Purchases.Commands.ReopenPurchase;

/// <summary>
/// Command to reopen a rejected purchase (Rejected -> Draft)
/// AC-1: Transition: Rejected (4) -> Draft (1)
/// AC-2: L'achat redevient modifiable
/// AC-3: RejectionReason precedent conserve dans l'historique
/// </summary>
public record ReopenPurchaseCommand(Guid PurchaseId, Guid BoutiqueId)
    : ICommand<ReopenPurchaseResult>;

public record ReopenPurchaseResult(Guid Id, string Status);

public class ReopenPurchaseCommandValidator : AbstractValidator<ReopenPurchaseCommand>
{
    public ReopenPurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty().WithMessage("L'identifiant de l'achat est obligatoire.");

        RuleFor(x => x.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
    }
}
