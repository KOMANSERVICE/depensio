using depensio.Application.UseCases.Sales.Commands.CreateSale;
using depensio.Application.UseCases.Sales.DTOs;
using depensio.Domain.ValueObjects;
using BuildingBlocks.Exceptions;

namespace depensio.Application.UseCases.Sales.Commands.CreateSale;

public class CreateSaleHandler(
    IDepensioDbContext _depensioRepository,
    IGenericRepository<Sale> _saleRepository,
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

        var sale = CreateNewSale(command.Sale);

        await _saleRepository.AddDataAsync(sale, cancellationToken);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);

        return new CreateSaleResult(sale.Id.Value);
    }

    private Sale CreateNewSale(SaleDTO saleDTO)
    {
        var saleId = SaleId.Of(Guid.NewGuid());

        return new Sale
        {
            Id = saleId,
            Date = DateTime.UtcNow,
            Title = saleDTO.Title,
            Description = saleDTO.Description,
            SaleItems = saleDTO.Items.Select(i => new SaleItem
            {
                Id = SaleItemId.Of(Guid.NewGuid()),
                ProductId = ProductId.Of(i.ProductId),
                Price = i.Price,
                Quantity = i.Quantity,
                SaleId = saleId
            }).ToList(),
            BoutiqueId = BoutiqueId.Of(saleDTO.BoutiqueId),
        };
    }
}
