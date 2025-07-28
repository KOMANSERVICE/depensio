namespace depensio.Application.Interfaces;

public interface IBoutiqueSettingService
{
    Task<T?> GetSettingAsync<T>(Guid boutiqueId, string key, T? defaultValue = default);
    Task SetSettingAsync<T>(Guid boutiqueId, string key, T value);
    Task<bool> HasSettingAsync(Guid boutiqueId, string key);
    Task RemoveSettingAsync(Guid boutiqueId, string key);
    Task<Dictionary<string, object>> GetAllSettingsAsync(Guid boutiqueId);
}
