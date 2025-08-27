using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Commands.CreateCodeBarre;

public record CreateCodeBarreCommand(ProductItemDTO ProductItem)
    : ICommand<CreateCodeBarreResult>;

public record CreateCodeBarreResult(BarcodeDTO Barcode);

public class CreateCodeBarreCommandValidator : AbstractValidator<CreateCodeBarreCommand>
{
    public CreateCodeBarreCommandValidator()
    {        
        RuleFor(x => x.ProductItem).NotNull().WithMessage("ProductItem is required");
        RuleFor(x => x.ProductItem.BarcodeCount).NotEmpty().WithMessage("BarcodeCount is required");
        RuleFor(x => x.ProductItem.BoutiqueId).NotEmpty().WithMessage("BoutiqueId is required");
    }
}
