namespace depensio.Application.UseCases.Sales.Commands.TransferSale;

/// <summary>
/// Command to manually transfer a validated sale to treasury
/// Used when a sale is validated but the CashFlow creation failed or was not done
/// Allows overriding the payment method, account, and category at transfer time
/// </summary>
public record TransferSaleCommand(
    Guid SaleId,
    Guid BoutiqueId,
    string? PaymentMethod = null,
    Guid? AccountId = null,
    string? CategoryId = null
) : ICommand<TransferSaleResult>;

public record TransferSaleResult(Guid Id, string Status, Guid? CashFlowId);

public class TransferSaleCommandValidator : AbstractValidator<TransferSaleCommand>
{
    public TransferSaleCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("L'identifiant de la vente est obligatoire.");

        RuleFor(x => x.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
    }
}
