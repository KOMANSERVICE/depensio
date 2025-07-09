using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Commands.CreateProduct;

public record CreateProductCommand(ProductDTO Product)
    : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {        
        RuleFor(x => x.Product).NotNull().WithMessage("Product is required");
        RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Product.Price).NotEmpty().WithMessage("Price is required");
        RuleFor(x => x.Product.CostPrice).NotEmpty().WithMessage("Cost Price is required");
        RuleFor(x => x.Product.BoutiqueId).NotEmpty().WithMessage("BoutiqueId is required");
    }
}
