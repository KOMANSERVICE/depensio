using BuildingBlocks.Exceptions;
using depensio.Application.Helpers;
using depensio.Application.Models;
using depensio.Application.UseCases.Sales.Commands.CreateSale;
using depensio.Application.UseCases.Sales.DTOs;
using depensio.Domain.Constants;
using depensio.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;

namespace depensio.Application.UseCases.Sales.Commands.CreateSale;

public class CreateSaleHandler(
    IDepensioDbContext _depensioRepository,
    IGenericRepository<Sale> _saleRepository,
    IBoutiqueSettingService _settingService,
    IUnitOfWork _unitOfWork
    )
    : ICommandHandler<CreateSaleCommand, CreateSaleResult>
{
    public async Task<CreateSaleResult> Handle(
        CreateSaleCommand command,
        CancellationToken cancellationToken
        )
    {


        var boutiqueExite = await _depensioRepository.Boutiques
            .AnyAsync(b => b.Id == BoutiqueId.Of(command.Sale.BoutiqueId), cancellationToken);
        if (!boutiqueExite)
        {
            throw new NotFoundException($"Boutique with ID {command.Sale.BoutiqueId} does not exist.", nameof(command.Sale.BoutiqueId));
        }

        // Récupère tous les IDs des produits dans la base
        var existingProductIds = await _depensioRepository.Products
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
        var products = _depensioRepository.Products
            .AsEnumerable() // ⚠️ déclenche la requête immédiatement
            .Where(p => productIds.Contains(p.Id.Value))
            .ToDictionary(p => p.Id.Value); // clé = Guid

        var autoSaisiePrix = await GetAutoSaisiePrixConfigAsync(saleDTO.BoutiqueId);
        // Étape 3: Construire la vente
        var saleItems = saleDTO.Items.Select(i =>
        {
            var productPrice = i.Price;

            if (!autoSaisiePrix){
                productPrice = products.TryGetValue(i.ProductId, out var product)
                ? product.Price
                : throw new InternalServerException($"Produit avec ID {i.ProductId} introuvable");
            }
               

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

}
