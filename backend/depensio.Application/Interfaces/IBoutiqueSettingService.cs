namespace depensio.Application.Interfaces;

public interface IBoutiqueSettingService
{
    Task<Guid> UpsertAsync(SettingDTO setting);
    Task<bool> HasSettingAsync(Guid boutiqueId, string key);
    Task<SettingDTO> GetSettingAsync(Guid boutiqueId, string key);
}
