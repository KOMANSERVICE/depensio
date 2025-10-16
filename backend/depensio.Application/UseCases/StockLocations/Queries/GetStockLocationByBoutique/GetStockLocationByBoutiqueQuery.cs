using depensio.Application.UseCases.StockLocations.DTOs;

namespace depensio.Application.UseCases.StockLocations.Queries.GetStockLocationByBoutique;

public record GetStockLocationByBoutiqueQuery(Guid BoutiqueId)
    : IQuery<GetStockLocationByBoutiqueResult>;

public record GetStockLocationByBoutiqueResult(IEnumerable<StockLocationDTO> StockLocations);

public class GetStockLocationByBoutiqueQueryValidator
    : AbstractValidator<GetStockLocationByBoutiqueQuery>
{
    public GetStockLocationByBoutiqueQueryValidator()
    {
        RuleFor(x => x.BoutiqueId)
            .NotEmpty().WithMessage("L'identifiant de la boutique est obligatoire.");
    }
}