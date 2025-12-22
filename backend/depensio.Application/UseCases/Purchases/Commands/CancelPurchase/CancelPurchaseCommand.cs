namespace depensio.Application.UseCases.Purchases.Commands.CancelPurchase;

/// <summary>
/// Command to cancel an approved purchase (Approved -> Cancelled)
/// AC-1: Transition: Approved (3) -> Cancelled (5)
/// AC-2: RejectionReason obligatoire (motif d'annulation)
/// AC-3: Si CashFlowId non null -> Appel annulation/contre-passation Tresorerie
/// </summary>
public record CancelPurchaseCommand(Guid PurchaseId, Guid BoutiqueId, string Reason)
    : ICommand<CancelPurchaseResult>;

public record CancelPurchaseResult(Guid Id, string Status);

public class CancelPurchaseCommandValidator : AbstractValidator<CancelPurchaseCommand>
{
    public CancelPurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty().WithMessage("L'identifiant de l'achat est obligatoire.");

        RuleFor(x => x.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");

        // AC-2: RejectionReason obligatoire (min 10 caracteres)
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Le motif d'annulation est obligatoire.")
            .MinimumLength(10).WithMessage("Le motif d'annulation doit contenir au moins 10 caractères.");
    }
}
