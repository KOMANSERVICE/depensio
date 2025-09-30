using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Commands.UpdateProductByBoutique;

public record UpdateProductByBoutiqueCommand(ProductUpdateDTO Product)
    : ICommand<UpdateProductByBoutiqueResult>;

public record UpdateProductByBoutiqueResult(Guid Id);

public class UpdateProductByBoutiqueCommandValidator : AbstractValidator<UpdateProductByBoutiqueCommand>
{
    public UpdateProductByBoutiqueCommandValidator()
    {        
        RuleFor(x => x.Product).NotNull().WithMessage("Product is required");
        RuleFor(x => x.Product.Id).NotEmpty().WithMessage("Id product is required");
        RuleFor(x => x.Product.Price).NotEmpty().WithMessage("Price is required");
        RuleFor(x => x.Product.CostPrice).NotEmpty().WithMessage("Cost Price is required");
        RuleFor(x => x.Product.BoutiqueId).NotEmpty().WithMessage("BoutiqueId is required");


        RuleFor(x => x.Product.Barcode)
            .Must(barcode => !string.IsNullOrWhiteSpace(barcode) || BoolHelper.IsValidEan13(barcode))
            .WithMessage("Le code-barres doit être un EAN-13 valide (13 chiffres avec clé de contrôle).");
    }
}
