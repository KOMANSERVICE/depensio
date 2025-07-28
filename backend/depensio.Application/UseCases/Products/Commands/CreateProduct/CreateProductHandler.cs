using depensio.Application.UseCases.Products.DTOs;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Products.Commands.CreateProduct;


public class CreateProductHandler(
    IGenericRepository<Product> _productRepository,
    IUnitOfWork _unitOfWork,
    IBarcodeService _barcodeService
    )
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken
        )
    {
        var product = await CreateNewProductAsync(command.Product);

        await _productRepository.AddDataAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new CreateProductResult(product.Id.Value);
    }

    private async Task<Product> CreateNewProductAsync(ProductDTO productDTO)
    {
        var productId = ProductId.Of(Guid.NewGuid());

        return new Product
        {
            Id = productId,
            Name = productDTO.Name,
            CostPrice = productDTO.CostPrice,
            Price = productDTO.Price,
            Barcode = await _barcodeService.GenerateBarcodeAsync(productDTO.BoutiqueId, productDTO.Barcode),
            Stock = productDTO.Stock,
            BoutiqueId = BoutiqueId.Of(productDTO.BoutiqueId),
        };
    }
}
