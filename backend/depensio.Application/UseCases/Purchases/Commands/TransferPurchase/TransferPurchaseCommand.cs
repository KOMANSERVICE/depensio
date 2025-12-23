namespace depensio.Application.UseCases.Purchases.Commands.TransferPurchase;

/// <summary>
/// Command to manually transfer an approved purchase to treasury
/// Used when a purchase is approved but the CashFlow creation failed or was not done
/// Allows overriding the payment method, account, and category at transfer time
/// </summary>
public record TransferPurchaseCommand(
    Guid PurchaseId,
    Guid BoutiqueId,
    string? PaymentMethod = null,
    Guid? AccountId = null,
    string? CategoryId = null
) : ICommand<TransferPurchaseResult>;

public record TransferPurchaseResult(Guid Id, string Status, Guid? CashFlowId);

public class TransferPurchaseCommandValidator : AbstractValidator<TransferPurchaseCommand>
{
    public TransferPurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty().WithMessage("L'identifiant de l'achat est obligatoire.");

        RuleFor(x => x.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
    }
}
