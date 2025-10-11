using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Commands.CreateProduct;

public record CreateProductCommand(ProductDTO Product)
    : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {        
        RuleFor(x => x.Product)
            .NotNull().WithMessage("Le produit est obligatoire.");
        
        RuleFor(x => x.Product.Name)
            .NotEmpty().WithMessage("Le nom du produit est obligatoire.");
        
        RuleFor(x => x.Product.Price)
            .NotEmpty().WithMessage("Le prix est obligatoire.");
        
        RuleFor(x => x.Product.CostPrice)
            .NotEmpty().WithMessage("Le prix de revient est obligatoire.");
        
        RuleFor(x => x.Product.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");

        RuleFor(x => x.Product.Barcode)
            .Must(barcode => string.IsNullOrWhiteSpace(barcode) || BoolHelper.IsValidEan13(barcode))
            .WithMessage("Le code-barres doit être un EAN-13 valide (13 chiffres avec clé de contrôle).");
    }
}
