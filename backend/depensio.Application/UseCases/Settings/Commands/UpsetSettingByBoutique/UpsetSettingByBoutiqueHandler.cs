using depensio.Domain.ValueObjects;

namespace depensio.Application.UserCases.Boutiques.Commands.UpsetSettingByBoutique;

public class UpsetSettingByBoutiqueHandler(
    IBoutiqueSettingService _boutiqueSettingService
    )
    : ICommandHandler<UpsetSettingByBoutiqueCommand, UpsetSettingByBoutiqueResult>
{
    public async Task<UpsetSettingByBoutiqueResult> Handle(
        UpsetSettingByBoutiqueCommand command,
        CancellationToken cancellationToken
        )
    {
        var settingId =  await _boutiqueSettingService.UpsertAsync(command.Setting);

        return new UpsetSettingByBoutiqueResult(settingId);
    }

}