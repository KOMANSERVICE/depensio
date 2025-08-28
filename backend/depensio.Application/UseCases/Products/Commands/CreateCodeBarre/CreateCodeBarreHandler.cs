using depensio.Application.UseCases.Products.DTOs;

namespace depensio.Application.UseCases.Products.Commands.CreateCodeBarre;


public class CreateCodeBarreHandler(
    IDepensioDbContext _dbContext,
    IGenericRepository<ProductItem> _productItemRepository,
    IUnitOfWork _unitOfWork,
    IBarcodeService _barcodeService,
    IUserContextService _userContextService,
    IProductService _productService
    )
    : ICommandHandler<CreateCodeBarreCommand, CreateCodeBarreResult>
{
    public async Task<CreateCodeBarreResult> Handle(
        CreateCodeBarreCommand request,
        CancellationToken cancellationToken
        )
    {
        var productItem = request.ProductItem;
        var userId = _userContextService.GetUserId();
        var boutiqueId = request.ProductItem.BoutiqueId;
        var productId = request.ProductItem.ProductId;
        var products = _dbContext.Boutiques
                   .Any(b => b.Id == BoutiqueId.Of(boutiqueId)
                               && b.UsersBoutiques.Any(ub => ub.UserId == userId)
                               && b.Products.Any(ub => ub.Id == ProductId.Of(productId)));
        if (!products) { 
            throw new UnauthorizedException("You are not authorized to add barcode to this product"); 
        }

        var product = await _productService.GetOneProductAsync(boutiqueId, productId);
        var codebar = await  _dbContext.ProductItems.Where(x => x.ProductId == ProductId.Of(productId) && x.Status == ProductStatus.Available).ToListAsync();
        if (productItem.BarcodeCount + codebar.Count > product?.Stock)
            throw new BadRequestException($"BarcodeCount cannot be greater than product stock ({product?.Stock - codebar.Count}).");


        var existingBarcodes = new HashSet<string>(
            await _dbContext.ProductItems.Select(p => p.Barcode).ToListAsync()
        );

        var newBarcodes = new List<string>();
        var productItems = new List<ProductItem>();
        for (int i = 0; i < productItem.BarcodeCount; i++)
        {
            string code;
            do
            {
                code = _barcodeService.GetBarcodeValue();
            } while (existingBarcodes.Contains(code));

            existingBarcodes.Add(code);
            newBarcodes.Add(code);

            productItems.Add(new ProductItem
            {
                Id = ProductItemId.Of(Guid.NewGuid()),
                ProductId = ProductId.Of(productItem.ProductId),
                Barcode = code
            });
        }


        await _productItemRepository.AddRangeDataAsync(productItems, cancellationToken);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new CreateCodeBarreResult(
            new BarcodeDTO(productId, newBarcodes)
        );
    }

}
