namespace depensio.Application.UseCases.Purchases.Commands.CancelPurchase;

/// <summary>
/// Command to cancel a purchase
/// US-PUR-008: Approved -> Cancelled (with Tresorerie call if CashFlowId exists)
/// US-PUR-009: Draft/PendingApproval/Rejected -> Cancelled (no Tresorerie call)
///
/// AC-1 (US-PUR-008): Transition: Approved (3) -> Cancelled (5)
/// AC-1 (US-PUR-009): Transitions: Draft (1) -> Cancelled (5), PendingApproval (2) -> Cancelled (5), Rejected (4) -> Cancelled (5)
/// AC-2 (US-PUR-008): RejectionReason obligatoire pour Approved
/// AC-2 (US-PUR-009): RejectionReason optionnel pour Draft/PendingApproval/Rejected
/// AC-3 (US-PUR-008): Si CashFlowId non null -> Appel annulation/contre-passation Tresorerie
/// AC-3 (US-PUR-009): Aucun appel Tresorerie (pas de CashFlow)
/// AC-4: Historique enregistré
/// </summary>
public record CancelPurchaseCommand(Guid PurchaseId, Guid BoutiqueId, string? Reason)
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

        // AC-2: RejectionReason optionnel mais si fourni, minimum 10 caractères
        // Note: La validation stricte (obligatoire) pour Approved est faite dans le handler
        RuleFor(x => x.Reason)
            .MinimumLength(10)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Le motif d'annulation doit contenir au moins 10 caractères.");
    }
}
