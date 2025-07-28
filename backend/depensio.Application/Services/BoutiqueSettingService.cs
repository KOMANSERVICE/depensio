using depensio.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace depensio.Application.Services;

public class BoutiqueSettingService(
    IGenericRepository<BoutiqueSetting> _repository,
    IUnitOfWork _unitOfWork,
    IMemoryCache _cache
    ) : IBoutiqueSettingService
{

    public Task<Dictionary<string, object>> GetAllSettingsAsync(Guid boutiqueId)
    {
        throw new NotImplementedException();
    }

    public async Task<T?> GetSettingAsync<T>(Guid boutiqueId, string key, T? defaultValue = default)
    {
        var cacheKey = $"boutique_setting_{boutiqueId}_{key}";

        if (_cache.TryGetValue(cacheKey, out T? cachedValue))
            return cachedValue;

        var setting = await _repository.FindAsync(b => b.BoutiqueId == BoutiqueId.Of(boutiqueId) && b.Key == key);

        if (setting == null)
            return defaultValue;

        var value = JsonSerializer.Deserialize<T>(setting.Value);
        _cache.Set(cacheKey, value, TimeSpan.FromMinutes(30));

        return value;
    }

    public Task<bool> HasSettingAsync(Guid boutiqueId, string key)
    {
        throw new NotImplementedException();
    }

    public Task RemoveSettingAsync(Guid boutiqueId, string key)
    {
        throw new NotImplementedException();
    }

    public async Task SetSettingAsync<T>(Guid boutiqueId, string key, T value)
    {
        var jsonValue = JsonSerializer.Serialize(value);
        await UpsertAsync(boutiqueId, key, jsonValue);
        await _unitOfWork.SaveChangesDataAsync();

        // Invalider le cache
        var cacheKey = $"boutique_setting_{boutiqueId}_{key}";
        _cache.Remove(cacheKey);
    }

    private async Task UpsertAsync(Guid boutiqueId, string key, string value)
    {
        // Rechercher le paramètre existant
        var existingSetting = await _repository.FindAsync(bs => bs.BoutiqueId == BoutiqueId.Of(boutiqueId) && bs.Key == key);

        if (existingSetting != null)
        {
            // Mise à jour du paramètre existant
            existingSetting.Value = value;
            _repository.UpdateData(existingSetting);
        }
        else
        {
            // Création d'un nouveau paramètre
            var newSetting = new BoutiqueSetting
            {
                BoutiqueId = BoutiqueId.Of(boutiqueId),
                Key = key,
                Value = value
            };

            await _repository.AddDataAsync(newSetting);
            existingSetting = newSetting;
        }

    }

}
