
using depensio.Application.UseCases.StockLocations.DTOs;
using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.StockLocations.Commands.CreateStockLocation;

public class CreateStockLocationHandler(
        IGenericRepository<StockLocation> _stockLocationRepository,
        IUnitOfWork _unitOfWork
    )
    : ICommandHandler<CreateStockLocationCommand, CreateStockLocationResult>
{
    public async Task<CreateStockLocationResult> Handle(CreateStockLocationCommand request, CancellationToken cancellationToken)
    {
        var boutiqueId = request.BoutiqueId;
        var stockLocationdto = request.StockLocation;

        var stockLocation = CreateStockLocation(boutiqueId, stockLocationdto);

        await _stockLocationRepository.AddDataAsync(stockLocation, cancellationToken);
        await _unitOfWork.SaveChangesDataAsync(cancellationToken);


        return new CreateStockLocationResult(stockLocation.Id.Value);
    }

    private StockLocation CreateStockLocation(Guid boutiqueId, StockLocationDTO stockLocationdto)
    {
        var stockLocationId = StockLocationId.Of(Guid.NewGuid());

        return new StockLocation
        {
            Id = stockLocationId,
            Name = stockLocationdto.Name,
            Address = stockLocationdto.Address,
            BoutiqueId = BoutiqueId.Of(boutiqueId)
        };

    }
}
