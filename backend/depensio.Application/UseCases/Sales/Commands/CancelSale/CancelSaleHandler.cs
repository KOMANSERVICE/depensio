using depensio.Application.ApiExterne.Tresoreries;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Sales.Commands.CancelSale;

/// <summary>
/// Handler for cancelling a sale
/// - Transition: Validated -> Cancelled
/// - If CashFlowId exists -> Call Treasury to delete CashFlow (contre-passation)
/// - Restore stock if not auto-managed
/// - Restore ProductItems to Available
/// </summary>
public class CancelSaleHandler(
    IDepensioDbContext _dbContext,
    IGenericRepository<Sale> _saleRepository,
    IGenericRepository<Product> _productRepository,
    IGenericRepository<ProductItem> _productItemRepository,
    IGenericRepository<SaleStatusHistory> _saleStatusHistoryRepository,
    IBoutiqueSettingService _settingService,
    IUnitOfWork _unitOfWork,
    ITresorerieService _tresorerieService,
    ILogger<CancelSaleHandler> _logger
    )
    : ICommandHandler<CancelSaleCommand, CancelSaleResult>
{
    public async Task<CancelSaleResult> Handle(
        CancelSaleCommand command,
        CancellationToken cancellationToken
        )
    {
        var sale = await _dbContext.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s => s.Id == SaleId.Of(command.SaleId), cancellationToken);

        if (sale == null)
        {
            throw new NotFoundException($"Vente avec ID {command.SaleId} introuvable.", nameof(command.SaleId));
        }

        if (sale.Status == SaleStatus.Cancelled)
        {
            throw new BadRequestException("Cette vente a déjà été annulée.");
        }

        // Treasury integration: Delete CashFlow if exists (contre-passation)
        if (sale.CashFlowId.HasValue)
        {
            try
            {
                var response = await _tresorerieService.DeleteCashFlowAsync(
                    sale.CashFlowId.Value,
                    "depensio",
                    sale.BoutiqueId.Value.ToString()
                );

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to delete CashFlow {CashFlowId} for Sale {SaleId}",
                        sale.CashFlowId.Value, sale.Id.Value);
                    // Don't block sale cancellation if Treasury call fails
                }
                else
                {
                    _logger.LogInformation("CashFlow {CashFlowId} deleted for Sale {SaleId}",
                        sale.CashFlowId.Value, sale.Id.Value);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the sale cancellation
                _logger.LogError(ex, "Error deleting CashFlow {CashFlowId} for Sale {SaleId}",
                    sale.CashFlowId, sale.Id.Value);
            }
        }

        var fromStatus = (int)sale.Status;
        sale.Status = SaleStatus.Cancelled;
        sale.CancelledAt = DateTime.UtcNow;
        sale.CancellationReason = command.Reason;
        sale.CashFlowId = null; // Clear CashFlowId since it has been deleted

        var stockIsAuto = await GetStockAuto(sale.BoutiqueId.Value);

        await RestoreStockAsync(sale, stockIsAuto, cancellationToken);
        await RestoreProductItemsAsync(sale, cancellationToken);

        // Add status history entry
        var statusHistory = new SaleStatusHistory
        {
            Id = SaleStatusHistoryId.Of(Guid.NewGuid()),
            SaleId = sale.Id,
            FromStatus = fromStatus,
            ToStatus = (int)SaleStatus.Cancelled,
            Comment = command.Reason
        };

        await _saleStatusHistoryRepository.AddDataAsync(statusHistory, cancellationToken);
        _saleRepository.UpdateData(sale);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Sale {SaleId} cancelled. Reason: {Reason}", sale.Id.Value, command.Reason ?? "(aucun motif)");

        return new CancelSaleResult(true);
    }

    private async Task RestoreStockAsync(Sale sale, bool stockIsAuto, CancellationToken cancellationToken)
    {
        if (stockIsAuto)
            return;

        var productIds = sale.SaleItems.Select(si => si.ProductId).Distinct().ToList();
        var products = await _dbContext.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        foreach (var saleItem in sale.SaleItems)
        {
            var product = products.FirstOrDefault(p => p.Id == saleItem.ProductId);
            if (product != null)
            {
                product.Stock += saleItem.Quantity;
                _productRepository.UpdateData(product);
            }
        }
    }

    private async Task RestoreProductItemsAsync(Sale sale, CancellationToken cancellationToken)
    {
        var saleItemIds = sale.SaleItems.Select(si => si.Id).ToList();

        var productItems = await _dbContext.ProductItems
            .Where(pi => pi.Status == ProductStatus.Sold)
            .ToListAsync(cancellationToken);

        var barcodes = await GetSaleBarcodes(sale, cancellationToken);

        var itemsToUpdate = productItems
            .Where(pi => barcodes.Contains(pi.Barcode))
            .ToList();

        foreach (var pi in itemsToUpdate)
        {
            pi.Status = ProductStatus.Available;
        }

        _productItemRepository.UpdateRangeData(itemsToUpdate);
    }

    private async Task<List<string>> GetSaleBarcodes(Sale sale, CancellationToken cancellationToken)
    {
        var productItemIds = sale.SaleItems.Select(si => si.ProductId).ToList();

        var productItems = await _dbContext.ProductItems
            .Where(pi => productItemIds.Contains(pi.ProductId) && pi.Status == ProductStatus.Sold)
            .Select(pi => pi.Barcode)
            .ToListAsync(cancellationToken);

        return productItems;
    }

    private async Task<bool> GetStockAuto(Guid boutiqueId)
    {
        var config = await _settingService.GetSettingAsync(
            boutiqueId,
            BoutiqueSettingKeys.PRODUCT_KEY
        );
        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(config.Value);
        var stockSetting = result?.FirstOrDefault(c => c.Id == BoutiqueSettingKeys.PRODUCT_STOCK_AUTOMATIQUE);
        return BoolHelper.ToBool(stockSetting?.Value.ToString());
    }
}
