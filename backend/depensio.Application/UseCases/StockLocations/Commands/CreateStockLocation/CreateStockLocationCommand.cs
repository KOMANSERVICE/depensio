using depensio.Application.UseCases.StockLocations.DTOs;

namespace depensio.Application.UseCases.StockLocations.Commands.CreateStockLocation;

public record CreateStockLocationCommand(Guid BoutiqueId,StockLocationDTO StockLocation)
    : ICommand<CreateStockLocationResult>;

public record CreateStockLocationResult(Guid Id);

public class CreateStockLocationCommandValidator
    : AbstractValidator<CreateStockLocationCommand>
{
    public CreateStockLocationCommandValidator()
    {
        RuleFor(x => x.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
        RuleFor(x => x.StockLocation)
            .NotNull().WithMessage("Le nom du stock est obligatoire.");
        RuleFor(x => x.StockLocation.Name)
            .NotEmpty().WithMessage("Le nom du stock est obligatoire.");
    }
}
