using depensio.Application.ApiExterne.Tresoreries;
using depensio.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Sales.Commands.CancelSale;

/// <summary>
/// Handler for cancelling a sale
/// US-TRS-003: Annuler une vente avec contre-passation via API reverse
/// - Transition: Validated -> Cancelled
/// - If CashFlowId exists -> Call ReverseCashFlowAsync endpoint for contra-entry (contre-passation)
/// - Store ReversalCashFlowId for audit trail
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

        // AC-5: Si CashFlowId existe -> Contre-passation Trésorerie via API reverse
        // Call ReverseCashFlowAsync endpoint to create a contra-entry (OUTFLOW) to reverse the original sale (INFLOW)
        Guid? reversalCashFlowId = null;
        if (sale.CashFlowId.HasValue)
        {
            reversalCashFlowId = await ReverseCashFlowAsync(sale, command.Reason);
        }

        // AC-7: Historique créé avec FromStatus=Validated, ToStatus=Cancelled
        var fromStatus = (int)sale.Status;
        sale.Status = SaleStatus.Cancelled;
        sale.CancelledAt = DateTime.UtcNow;
        sale.CancellationReason = command.Reason;
        // Note: We keep CashFlowId as reference to the original CashFlow

        // Store the reversal CashFlowId for audit trail (contre-passation)
        if (reversalCashFlowId.HasValue)
        {
            sale.ReversalCashFlowId = reversalCashFlowId;
        }

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
    /// AC-5: Create contra-entry (contre-passation) in Treasury via ReverseCashFlowAsync
    /// Original sale was INFLOW, contra-entry is OUTFLOW to reverse it
    /// Uses POST /api/cash-flows/{id}/reverse endpoint
    /// </summary>
    private async Task<Guid?> ReverseCashFlowAsync(Sale sale, string? cancellationReason)
    {
        try
        {
            var reverseRequest = new ReverseCashFlowRequest(
                Reason: cancellationReason ?? "Annulation vente",
                SourceType: "Sale",
                SourceId: sale.Id.Value
            );

            var response = await _tresorerieService.ReverseCashFlowAsync(
                sale.CashFlowId!.Value,
                "depensio",
                sale.BoutiqueId.Value.ToString(),
                reverseRequest
            );

            if (!response.Success || response.Data == null)
            {
                _logger.LogError("Failed to reverse CashFlow {CashFlowId} for Sale {SaleId}. Message: {Message}",
                    sale.CashFlowId.Value, sale.Id.Value, response.Message);
                throw new ExternalServiceException("Tresorerie",
                    $"Échec de la contre-passation du mouvement de trésorerie associé. Veuillez réessayer.");
            }

            var reversalCashFlowId = response.Data.ReversalCashFlowId;
            _logger.LogInformation("CashFlow {CashFlowId} reversed for Sale {SaleId}. Reversal CashFlowId: {ReversalCashFlowId}",
                sale.CashFlowId.Value, sale.Id.Value, reversalCashFlowId);

            return reversalCashFlowId;
        }
        catch (ExternalServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reversing CashFlow {CashFlowId} for Sale {SaleId}",
                sale.CashFlowId, sale.Id.Value);
            throw new ExternalServiceException("Tresorerie",
                $"Erreur lors de la contre-passation du mouvement de trésorerie: {ex.Message}", ex);
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
