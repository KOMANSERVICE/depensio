using depensio.Application.ApiExterne.Tresoreries;
using depensio.Application.UseCases.Sales.DTOs;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Sales.Commands.CreateSale;

public class CreateSaleHandler(
    IDepensioDbContext _dbContext,
    IGenericRepository<Sale> _saleRepository,
    IGenericRepository<Product> _productRepository,
    IGenericRepository<ProductItem> _productItemRepository,
    IBoutiqueSettingService _settingService,
    IUnitOfWork _unitOfWork,
    IProductService _productService,
    ITresorerieService _tresorerieService,
    ILogger<CreateSaleHandler> _logger
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

        // Create CashFlow if automatic treasury sending is enabled and payment info is provided
        var envoiAutoEnabled = await GetEnvoiAutomatiqueConfigAsync(command.Sale.BoutiqueId);
        if (envoiAutoEnabled && command.Sale.AccountId.HasValue)
        {
            await CreateCashFlowFromSaleAsync(sale, command.Sale.BoutiqueId);
        }

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

        // Calculate total amount from items
        decimal totalAmount = 0;

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

            // Add to total amount
            totalAmount += productPrice * i.Quantity;

            return new SaleItem
            {
                Id = SaleItemId.Of(Guid.NewGuid()),
                ProductId = ProductId.Of(i.ProductId),
                Price = productPrice,
                Quantity = i.Quantity,
                SaleId = saleId
            };
        }).ToList();

        // Create initial status history entry (FromStatus = null, ToStatus = Validated)
        var statusHistory = new SaleStatusHistory
        {
            Id = SaleStatusHistoryId.Of(Guid.NewGuid()),
            SaleId = saleId,
            FromStatus = null,
            ToStatus = (int)SaleStatus.Validated,
            Comment = "Vente créée et validée"
        };

        return new Sale
        {
            Id = saleId,
            Date = DateTime.UtcNow,
            BoutiqueId = BoutiqueId.Of(saleDTO.BoutiqueId),
            SaleItems = saleItems,
            TotalAmount = totalAmount,
            // Treasury integration fields (optional)
            PaymentMethodId = saleDTO.PaymentMethodId,
            AccountId = saleDTO.AccountId,
            CategoryId = saleDTO.CategoryId,
            StatusHistory = new List<SaleStatusHistory> { statusHistory }
        };
    }

    /// <summary>
    /// Call Trésorerie service to create CashFlow from sale (INFLOW)
    /// </summary>
    private async Task CreateCashFlowFromSaleAsync(Sale sale, Guid boutiqueId)
    {
        try
        {
            var request = new CreateCashFlowFromSaleRequest(
                SaleId: sale.Id.Value,
                SaleReference: $"VTE-{sale.Id.Value.ToString()[..8].ToUpper()}",
                Amount: sale.TotalAmount,
                AccountId: sale.AccountId!.Value,
                PaymentMethod: sale.PaymentMethodId?.ToString() ?? "CASH",
                SaleDate: sale.Date,
                CustomerName: null,
                CustomerId: null,
                CategoryId: sale.CategoryId?.ToString()
            );

            var result = await _tresorerieService.CreateCashFlowFromSaleAsync(
                "depensio",
                boutiqueId.ToString(),
                request
            );

            if (result.Success && result.Data != null)
            {
                // Update sale with CashFlowId
                sale.CashFlowId = result.Data.CashFlow.Id;
                await _unitOfWork.SaveChangesDataAsync(default);
                _logger.LogInformation("CashFlow {CashFlowId} created for Sale {SaleId}", result.Data.CashFlow.Id, sale.Id.Value);
            }
            else
            {
                _logger.LogWarning("Failed to create CashFlow for Sale {SaleId}: {Message}", sale.Id.Value, result.Message);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the sale creation
            _logger.LogError(ex, "Error creating CashFlow for Sale {SaleId}", sale.Id.Value);
        }
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

    private async Task<bool> GetEnvoiAutomatiqueConfigAsync(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
            boutiqueId,
            BoutiqueSettingKeys.VENTE_KEY
        );

        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);
        var envoiAutoSetting = result?.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.VENTE_ENVOI_AUTOMATIQUE_TRESORERIE);

        return BoolHelper.ToBool(envoiAutoSetting?.Value?.ToString());
    }

}
