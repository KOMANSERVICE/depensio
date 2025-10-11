using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Commands.CreateCodeBarre;

public record CreateCodeBarreCommand(ProductItemDTO ProductItem)
    : ICommand<CreateCodeBarreResult>;

public record CreateCodeBarreResult(BarcodeDTO Barcode);

public class CreateCodeBarreCommandValidator : AbstractValidator<CreateCodeBarreCommand>
{
    public CreateCodeBarreCommandValidator()
    {        
        RuleFor(x => x.ProductItem)
            .NotNull().WithMessage("L'article du produit est obligatoire.");
        
        RuleFor(x => x.ProductItem.BarcodeCount)
            .NotEmpty().WithMessage("Le nombre de codes-barres est obligatoire.");
        
        RuleFor(x => x.ProductItem.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
    }
}
