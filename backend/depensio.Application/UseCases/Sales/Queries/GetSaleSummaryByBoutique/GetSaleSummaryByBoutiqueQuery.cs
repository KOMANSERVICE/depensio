using depensio.Application.UseCases.Sales.DTOs;

namespace depensio.Application.UseCases.Sales.Queries.GetSaleSummaryByBoutique;
public record GetSaleSummaryByBoutiqueQuery(Guid BoutiqueId)
    : IQuery<GetSaleSummaryByBoutiqueResult>;
public record GetSaleSummaryByBoutiqueResult(IEnumerable<SaleSummaryDTO> SaleSummarys);
