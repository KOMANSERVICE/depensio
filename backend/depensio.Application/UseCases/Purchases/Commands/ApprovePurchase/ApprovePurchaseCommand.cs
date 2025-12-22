namespace depensio.Application.UseCases.Purchases.Commands.ApprovePurchase;

/// <summary>
/// Command to approve a purchase pending approval (PendingApproval -> Approved)
/// This will create a CashFlow in the Treasury service
/// </summary>
public record ApprovePurchaseCommand(Guid PurchaseId, Guid BoutiqueId)
    : ICommand<ApprovePurchaseResult>;

public record ApprovePurchaseResult(Guid Id, string Status, Guid? CashFlowId);

public class ApprovePurchaseCommandValidator : AbstractValidator<ApprovePurchaseCommand>
{
    public ApprovePurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty().WithMessage("L'identifiant de l'achat est obligatoire.");

        RuleFor(x => x.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
    }
}
