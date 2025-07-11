using depensio.Application.UseCases.Sales.DTOs;

namespace depensio.Application.UseCases.Sales.Queries.GetSaleByBoutique;
public record GetSaleByBoutiqueQuery(Guid BoutiqueId)
    : IQuery<GetSaleByBoutiqueResult>;
public record GetSaleByBoutiqueResult(IEnumerable<SaleDTO> Sales);
