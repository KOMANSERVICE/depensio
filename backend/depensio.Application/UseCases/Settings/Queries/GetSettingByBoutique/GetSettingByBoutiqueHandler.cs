using depensio.Domain.Settings;

namespace depensio.Application.UseCases.Settings.Queries.GetSettingByBoutique;

public class GetSettingByBoutiqueHandler(
    IBoutiqueSettingService _boutiqueSettingService)
    : IQueryHandler<GetSettingByBoutiqueQuery, GetSettingByBoutiqueResult>
{
    public async Task<GetSettingByBoutiqueResult> Handle(GetSettingByBoutiqueQuery request, CancellationToken cancellationToken)
    {

        var sales = await _boutiqueSettingService.GetSettingAsync(request.BoutiqueId, request.Key);

        return new GetSettingByBoutiqueResult(sales);
    }
}
