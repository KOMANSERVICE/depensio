using depensio.Shared.Pages.Dashboards.Models;

namespace depensio.Shared.Services;

public interface IDashboardServices
{
    [Post("/dashboard/{boutiqueId}/saledetail")]
    Task<BaseResponse<GetSalesDetailByBoutiqueResponse>> GetSalesDetailByBoutiqueAsync(Guid boutiqueId, GetSalesDetailByBoutiqueRequest request);
}
