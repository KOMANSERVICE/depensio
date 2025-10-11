using depensio.Application.UseCases.Sales.DTOs;

namespace depensio.Application.UseCases.Sales.Commands.CreateSale;

public record CreateSaleCommand(SaleDTO Sale)
    : ICommand<CreateSaleResult>;

public record CreateSaleResult(Guid Id);

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.Sale)
            .NotNull().WithMessage("La vente est obligatoire.");
        
        RuleFor(x => x.Sale.Items)
            .NotEmpty().WithMessage("Les articles sont obligatoires.");
        
        RuleFor(x => x.Sale.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
    }
}
