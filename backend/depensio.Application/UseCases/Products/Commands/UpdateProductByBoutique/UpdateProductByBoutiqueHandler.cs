using depensio.Application.Interfaces;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace depensio.Application.UseCases.Products.Commands.UpdateProductByBoutique;


public class UpdateProductByBoutiqueHandler(
    IGenericRepository<Product> _productRepository,
    IDepensioDbContext dbContext,
    IUserContextService _userContextService,
    IUnitOfWork _unitOfWork
    )
    : ICommandHandler<UpdateProductByBoutiqueCommand, UpdateProductByBoutiqueResult>
{
    public async Task<UpdateProductByBoutiqueResult> Handle(
        UpdateProductByBoutiqueCommand command,
        CancellationToken cancellationToken
        )
    {

        var userId = _userContextService.GetUserId();
        var productId = ProductId.Of(command.Product.Id);

        var productExist = await dbContext.Boutiques
                    .AnyAsync(b => b.Id == BoutiqueId.Of(command.Product.BoutiqueId)
                                && b.UsersBoutiques.Any(ub => ub.UserId == userId)
                                && b.Products.Any(p => p.Id == productId));

        if(!productExist)
            throw new InternalServerException("Product not found or you don't have access to this product.");



        var product = await UpdateProductAsync(command.Product);

        _productRepository.UpdateData(product, cancellationToken);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new UpdateProductByBoutiqueResult(product.Id.Value);
    }

    private async Task<Product> UpdateProductAsync(ProductUpdateDTO productDTO)
    {
        return new Product
        {
            Id = ProductId.Of(productDTO.Id),
            BoutiqueId = BoutiqueId.Of(productDTO.BoutiqueId),
            Name = productDTO.Name,
            CostPrice = productDTO.CostPrice,
            Price = productDTO.Price,
            Stock = productDTO.Stock
        };
    }
}
