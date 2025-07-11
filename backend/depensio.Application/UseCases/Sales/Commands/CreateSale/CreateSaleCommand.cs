using depensio.Application.UseCases.Sales.DTOs;

namespace depensio.Application.UseCases.Sales.Commands.CreateSale;

public record CreateSaleCommand(SaleDTO Sale)
    : ICommand<CreateSaleResult>;

public record CreateSaleResult(Guid Id);

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.Sale).NotNull().WithMessage("Sale is required");
        RuleFor(x => x.Sale.Items).NotEmpty().WithMessage("Items is required");
        RuleFor(x => x.Sale.BoutiqueId).NotEmpty().WithMessage("BoutiqueId is required");
        
    }
}
