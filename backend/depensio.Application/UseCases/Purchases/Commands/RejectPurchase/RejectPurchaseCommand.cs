namespace depensio.Application.UseCases.Purchases.Commands.RejectPurchase;

/// <summary>
/// Command to reject a purchase pending approval (PendingApproval -> Rejected)
/// AC-1: Transition: PendingApproval (2) -> Rejected (4)
/// AC-2: RejectionReason obligatoire (min 10 caracteres)
/// AC-3: Aucun appel au service Tresorerie
/// </summary>
public record RejectPurchaseCommand(Guid PurchaseId, Guid BoutiqueId, string RejectionReason)
    : ICommand<RejectPurchaseResult>;

public record RejectPurchaseResult(Guid Id, string Status);

public class RejectPurchaseCommandValidator : AbstractValidator<RejectPurchaseCommand>
{
    public RejectPurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty().WithMessage("L'identifiant de l'achat est obligatoire.");

        RuleFor(x => x.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");

        // AC-2: RejectionReason obligatoire (min 10 caracteres)
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("Le motif de rejet est obligatoire.")
            .MinimumLength(10).WithMessage("Le motif de rejet doit contenir au moins 10 caractères.");
    }
}
