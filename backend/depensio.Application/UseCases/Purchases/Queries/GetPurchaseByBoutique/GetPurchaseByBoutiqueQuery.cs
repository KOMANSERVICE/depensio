using depensio.Application.UseCases.Purchases.DTOs;

namespace depensio.Application.UseCases.Purchases.Queries.GetPurchaseByBoutique;
public record GetPurchaseByBoutiqueQuery(Guid BoutiqueId)
    : IQuery<GetPurchaseByBoutiqueResult>;
public record GetPurchaseByBoutiqueResult(IEnumerable<PurchaseDTO> Purchases);
