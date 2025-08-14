using depensio.Application.Helpers;
using depensio.Application.Interfaces;
using depensio.Application.Models;
using depensio.Application.Services;
using depensio.Application.UseCases.Products.DTOs;
using depensio.Domain.Constants;
using depensio.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace depensio.Application.UseCases.Products.Commands.UpdateProductByBoutique;


public class UpdateProductByBoutiqueHandler(
    IGenericRepository<Product> _productRepository,
    IDepensioDbContext dbContext,
    IUserContextService _userContextService,
    IUnitOfWork _unitOfWork,
    IBoutiqueSettingService _settingService
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
        var stockIsAuto = await AutoriserLesProduitAvecStockZero(productDTO.BoutiqueId);
        var produt = await _productRepository.FindAsync(p => p.Id == ProductId.Of(productDTO.Id));
        produt.CostPrice = productDTO.CostPrice;
        produt.Price = productDTO.Price;
        if(!stockIsAuto)
            produt.Stock = productDTO.Stock;


        return produt;
    }

    private async Task<bool> AutoriserLesProduitAvecStockZero(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
                  boutiqueId,
                  BoutiqueSettingKeys.PRODUCT_KEY
              );

        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);

        var stockAuto = result?.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE);

        return BoolHelper.ToBool(stockAuto?.Value.ToString());
    }
}
