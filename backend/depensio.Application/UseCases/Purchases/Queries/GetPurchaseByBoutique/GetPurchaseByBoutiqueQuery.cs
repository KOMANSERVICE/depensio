using depensio.Application.UseCases.Purchases.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace depensio.Application.UseCases.Purchases.Queries.GetPurchaseByBoutique;
public record GetPurchaseByBoutiqueQuery(Guid BoutiqueId)
    : IQuery<GetPurchaseByBoutiqueResult>;
public record GetPurchaseByBoutiqueResult(IEnumerable<PurchaseDTO> Purchases);
