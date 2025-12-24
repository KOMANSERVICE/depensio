using depensio.Application.ApiExterne.Tresoreries;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Sales.Commands.CancelSale;

/// <summary>
/// Handler for cancelling a sale
/// US-SAL-002: Annuler une vente avec contre-passation
/// - Transition: Validated -> Cancelled
/// - If CashFlowId exists -> Create contra-entry (contre-passation) in Treasury
/// - Restore stock if not auto-managed
/// - Restore ProductItems to Available
/// - Create status history with FromStatus=Validated, ToStatus=Cancelled
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

        // AC-5: Si CashFlowId existe -> Contre-passation Trésorerie
        // Create a contra-entry (EXPENSE) to reverse the original sale (INCOME)
        if (sale.CashFlowId.HasValue && sale.AccountId.HasValue && !string.IsNullOrEmpty(sale.CategoryId))
        {
            await CreateContraEntryAsync(sale, command.Reason);
        }

        // AC-7: Historique créé avec FromStatus=Validated, ToStatus=Cancelled
        var fromStatus = (int)sale.Status;
        sale.Status = SaleStatus.Cancelled;
        sale.CancelledAt = DateTime.UtcNow;
        sale.CancellationReason = command.Reason;
        // Note: We keep CashFlowId as reference to the original CashFlow

        // AC-3 & AC-4: Restore stock and ProductItems
        var stockIsAuto = await GetStockAuto(sale.BoutiqueId.Value);
        await RestoreStockAsync(sale, stockIsAuto, cancellationToken);
        await RestoreProductItemsAsync(sale, cancellationToken);

        // AC-7: Add status history entry with FromStatus and ToStatus
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

    /// <summary>
    /// AC-5: Create contra-entry (contre-passation) in Treasury
    /// Original sale was INCOME, contra-entry is EXPENSE to reverse it
    /// </summary>
    private async Task CreateContraEntryAsync(Sale sale, string? cancellationReason)
    {
        try
        {
            var contraEntryRequest = new CreateCashFlowRequest(
                Type: CashFlowTypeExtended.EXPENSE,
                CategoryId: sale.CategoryId!,
                Label: $"Annulation vente VTE-{sale.Id.Value.ToString()[..8].ToUpper()}",
                Description: $"Contre-passation suite à annulation. {(string.IsNullOrWhiteSpace(cancellationReason) ? "" : $"Motif: {cancellationReason}")}",
                Amount: sale.TotalAmount,
                AccountId: sale.AccountId!.Value,
                PaymentMethod: sale.PaymentMethodId?.ToString() ?? "CASH",
                Date: DateTime.UtcNow,
                CustomerName: null,
                SupplierName: null,
                AttachmentUrl: null
            );

            var result = await _tresorerieService.CreateCashFlowAsync(
                "depensio",
                sale.BoutiqueId.Value.ToString(),
                contraEntryRequest
            );

            if (result.Success && result.Data != null)
            {
                _logger.LogInformation(
                    "Contra-entry CashFlow {ContraCashFlowId} created for cancelled Sale {SaleId}. Original CashFlow: {OriginalCashFlowId}",
                    result.Data.CashFlow.Id, sale.Id.Value, sale.CashFlowId);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to create contra-entry CashFlow for Sale {SaleId}: {Message}",
                    sale.Id.Value, result.Message);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the sale cancellation
            _logger.LogError(ex,
                "Error creating contra-entry CashFlow for Sale {SaleId}. Original CashFlow: {CashFlowId}",
                sale.Id.Value, sale.CashFlowId);
        }
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
