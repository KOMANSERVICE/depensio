using depensio.Application.UseCases.Sales.DTOs;

namespace depensio.Application.UseCases.Sales.Queries.GetSaleDetailByBoutique;
public record GetSaleDetailByBoutiqueQuery(Guid BoutiqueId)
    : IQuery<GetSaleDetailByBoutiqueResult>;
public record GetSaleDetailByBoutiqueResult(IEnumerable<SaleDetailDTO> Sales);
