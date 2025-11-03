using depensio.Application.UseCases.Sales.DTOs;
using System.Text.Json;

namespace depensio.Application.UseCases.Sales.Commands.CreateSale;

public class CreateSaleHandler(
    IDepensioDbContext _dbContext,
    IGenericRepository<Sale> _saleRepository,
    IGenericRepository<Product> _productRepository,
    IGenericRepository<ProductItem> _productItemRepository,
    IBoutiqueSettingService _settingService,
    IUnitOfWork _unitOfWork,
    IProductService _productService
    )
    : ICommandHandler<CreateSaleCommand, CreateSaleResult>
{
    public async Task<CreateSaleResult> Handle(
        CreateSaleCommand command,
        CancellationToken cancellationToken
        )
    {


        var boutiqueExite = await _dbContext.Boutiques
            .AnyAsync(b => b.Id == BoutiqueId.Of(command.Sale.BoutiqueId), cancellationToken);
        if (!boutiqueExite)
        {
            throw new NotFoundException($"Boutique with ID {command.Sale.BoutiqueId} does not exist.", nameof(command.Sale.BoutiqueId));
        }

        // Récupère tous les IDs des produits dans la base
        var existingProductIds = await _dbContext.Products
            .Select(p => p.Id.Value)
            .ToListAsync(cancellationToken);

        // Filtre les items du saleDTO qui ne sont pas encore dans la base
        var nonExistentItems = command.Sale.Items
            .Any(item => !existingProductIds.Contains(item.ProductId));
        if (nonExistentItems)
        {
            throw new NotFoundException($"Any product does not exist.", nameof(command.Sale.BoutiqueId));
        }

        var sale = await CreateNewSaleAsync(command.Sale);
        var productItems = await UpdateProductItemAsync(command.Sale);

        _productItemRepository.UpdateRangeData(productItems);
        await _saleRepository.AddDataAsync(sale, cancellationToken);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new CreateSaleResult(sale.Id.Value);
    }

    private async Task<Sale> CreateNewSaleAsync(SaleDTO saleDTO)
    {
        var saleId = SaleId.Of(Guid.NewGuid());

        // Étape 1: Récupérer les ID des produits demandés
        var productIds = saleDTO.Items.Select(i => i.ProductId).Distinct().ToList();
        // Étape 2: Charger les produits depuis le contexte (tu dois avoir un dbContext ici)

        var stockIsAuto = await GetStockAuto(saleDTO.BoutiqueId);
        var productResult = await _productService.GetProductsAsync(saleDTO.BoutiqueId);
        var products = productResult // ⚠️ déclenche la requête immédiatement
            .Where(p => productIds.Contains(p.Id.Value))
            .ToDictionary(p => p.Id.Value); // clé = Guid

        var autoSaisiePrix = await GetAutoSaisiePrixConfigAsync(saleDTO.BoutiqueId);
        var autoVenteStockZero = await GetAutoVenteStockZeroConfigAsync(saleDTO.BoutiqueId);
        // Étape 3: Construire la vente
        var saleItems = saleDTO.Items.Select(i =>
        {
            if (!products.TryGetValue(i.ProductId, out var product))
                throw new InternalServerException($"Produit avec ID {i.ProductId} introuvable");

            // Vérifier le prix
            var productPrice = autoSaisiePrix ? i.Price : product.Price;

            // Vérifier le stock si autoVenteStockZero est désactivé
            if (!autoVenteStockZero && i.Quantity > product.Stock)
            {
                throw new BadRequestException(
                    $"Stock insuffisant pour le produit '{product.Name}'. " +
                    $"Demandé: {i.Quantity}, Disponible: {product.Stock}"
                );
            }

            if(!stockIsAuto)
                product.Stock -= i.Quantity; // Mettre à jour le stock du produit

            _productRepository.UpdateData(product);

            return new SaleItem
            {
                Id = SaleItemId.Of(Guid.NewGuid()),
                ProductId = ProductId.Of(i.ProductId),
                Price = productPrice, // TODO a revoir
                Quantity = i.Quantity,
                SaleId = saleId
            };
        }).ToList();

        return new Sale
        {
            Id = saleId,
            Date = DateTime.UtcNow,
            BoutiqueId = BoutiqueId.Of(saleDTO.BoutiqueId),
            SaleItems = saleItems
        };
    }

    private async Task<List<ProductItem>> UpdateProductItemAsync(SaleDTO saleDTO)
    {
        // reccuperation des code barres
        var allBarcodes = saleDTO.Items
        .SelectMany(item => item.Barcodes)
        .Distinct()
        .ToList();

        var productItems = await _dbContext.ProductItems
        .Where(pi => allBarcodes.Contains(pi.Barcode))
        .ToListAsync();

        // Mise à jour du statut
        foreach (var pi in productItems)
        {
            pi.Status = ProductStatus.Sold; // ou pi.IsSold = true;
        }

        return productItems;
    }
    private async Task<bool> GetAutoSaisiePrixConfigAsync(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
            boutiqueId,
            BoutiqueSettingKeys.VENTE_KEY
        );

        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);
        var autoSaisiePrix = result.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.VENTE_AUTORISER_MODIFICATION_PRIX);

        return BoolHelper.ToBool(autoSaisiePrix?.Value.ToString());
    }

    private async Task<bool> GetAutoVenteStockZeroConfigAsync(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
            boutiqueId,
            BoutiqueSettingKeys.VENTE_KEY
        );

        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);
        var autoVenteStockZero = result.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.VENTE_AUTORISER_VENTE_AVEC_STOCK_ZERO);

        return BoolHelper.ToBool(autoVenteStockZero?.Value.ToString());
    }

    private async Task<bool> GetStockAuto(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
            boutiqueId,
            BoutiqueSettingKeys.PRODUCT_KEY
        );
        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);
        var stockSetting = result.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE);
        return BoolHelper.ToBool(stockSetting?.Value.ToString());
    }

}
