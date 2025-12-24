using depensio.Application.ApiExterne.Tresoreries;
using depensio.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace depensio.Application.UseCases.Sales.Commands.TransferSale;

/// <summary>
/// Handler for manually transferring a validated sale to treasury
/// Used when a sale is validated but the CashFlow creation failed or was not done
/// </summary>
public class TransferSaleHandler(
    IDepensioDbContext _depensioDbContext,
    IUnitOfWork _unitOfWork,
    IUserContextService _userContextService,
    ITresorerieService _tresorerieService,
    IGenericRepository<SaleStatusHistory> _saleStatusHistoryRepository,
    IGenericRepository<Sale> _saleRepository,
    ILogger<TransferSaleHandler> _logger
    )
    : ICommandHandler<TransferSaleCommand, TransferSaleResult>
{
    public async Task<TransferSaleResult> Handle(
        TransferSaleCommand command,
        CancellationToken cancellationToken
        )
    {
        var saleId = SaleId.Of(command.SaleId);
        var boutiqueId = BoutiqueId.Of(command.BoutiqueId);

        // Retrieve sale with items
        var sale = await _depensioDbContext.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s => s.Id == saleId && s.BoutiqueId == boutiqueId, cancellationToken);

        if (sale == null)
        {
            throw new NotFoundException($"La vente avec l'ID {command.SaleId} n'existe pas.", nameof(command.SaleId));
        }

        // Verify current status is Validated (not Cancelled)
        var currentStatus = (SaleStatus)sale.Status;
        if (currentStatus == SaleStatus.Cancelled)
        {
            throw new BadRequestException("Impossible de transférer une vente annulée.");
        }

        // Check if already transferred
        if (sale.CashFlowId.HasValue)
        {
            throw new BadRequestException("Cette vente a déjà été transférée à la trésorerie.");
        }

        // Use provided values or fall back to existing sale values
        var accountId = command.AccountId ?? sale.AccountId;
        var paymentMethod = !string.IsNullOrWhiteSpace(command.PaymentMethod) ? command.PaymentMethod : sale.PaymentMethodId;
        var categoryId = !string.IsNullOrWhiteSpace(command.CategoryId) ? command.CategoryId : sale.CategoryId;

        // Validate required fields for Treasury call
        if (!accountId.HasValue)
        {
            throw new BadRequestException("Le compte est obligatoire pour transférer la vente.");
        }

        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            throw new BadRequestException("Le mode de paiement est obligatoire pour transférer la vente.");
        }

        if (string.IsNullOrWhiteSpace(categoryId))
        {
            throw new BadRequestException("La catégorie est obligatoire pour transférer la vente.");
        }

        // Update sale with the values that will be used for transfer
        sale.AccountId = accountId;
        sale.PaymentMethodId = paymentMethod;
        sale.CategoryId = categoryId;

        var userId = _userContextService.GetUserId();

        // Call ITresorerieService.CreateCashFlowFromSaleAsync()
        Guid? cashFlowId = null;
        try
        {
            var request = new CreateCashFlowFromSaleRequest(
                SaleId: sale.Id.Value,
                SaleReference: $"VTE-{sale.Id.Value.ToString()[..8].ToUpper()}",
                Amount: sale.TotalAmount,
                AccountId: accountId!.Value,
                PaymentMethod: paymentMethod!,
                SaleDate: sale.Date,
                CustomerName: null,
                CustomerId: null,
                CategoryId: categoryId
            );

            var result = await _tresorerieService.CreateCashFlowFromSaleAsync(
                "depensio",
                command.BoutiqueId.ToString(),
                request
            );

            if (result.Success && result.Data != null)
            {
                cashFlowId = result.Data.CashFlow.Id;
                _logger.LogInformation("CashFlow {CashFlowId} created for Sale {SaleId} via manual transfer", cashFlowId, sale.Id.Value);
            }
            else
            {
                _logger.LogError("Failed to create CashFlow for Sale {SaleId} via manual transfer: {Message}", sale.Id.Value, result.Message);
                throw new ExternalServiceException("Tresorerie", $"Échec de l'appel au service de trésorerie: {result.Message}");
            }
        }
        catch (ExternalServiceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating CashFlow for Sale {SaleId} via manual transfer, AccountId: {AccountId}, Amount: {Amount}",
                sale.Id.Value, sale.AccountId, sale.TotalAmount);
            throw new ExternalServiceException("Tresorerie", $"Erreur lors de l'appel au service de trésorerie: {ex.Message}", ex);
        }

        // Update sale with CashFlowId
        sale.CashFlowId = cashFlowId;

        // Record history entry for the transfer
        var statusHistory = new SaleStatusHistory
        {
            Id = SaleStatusHistoryId.Of(Guid.NewGuid()),
            SaleId = sale.Id,
            FromStatus = (int)SaleStatus.Validated,
            ToStatus = (int)SaleStatus.Validated, // Status doesn't change
            Comment = "Transfert manuel vers la trésorerie"
        };

        await _saleStatusHistoryRepository.AddDataAsync(statusHistory, cancellationToken);
        _saleRepository.UpdateData(sale);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        _logger.LogInformation("Sale {SaleId} manually transferred to treasury by user {UserId}, CashFlowId: {CashFlowId}",
            sale.Id.Value, userId, cashFlowId);

        return new TransferSaleResult(sale.Id.Value, "Transferred", cashFlowId);
    }
}
