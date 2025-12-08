namespace depensio.Application.UseCases.Sales.Commands.CancelSale;

public record CancelSaleCommand(Guid SaleId, string? Reason) : ICommand<CancelSaleResult>;

public record CancelSaleResult(bool Success);

public class CancelSaleCommandValidator : AbstractValidator<CancelSaleCommand>
{
    public CancelSaleCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("L'identifiant de la vente est obligatoire.");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("La raison ne peut pas dépasser 500 caractères.");
    }
}
