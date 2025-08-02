using depensio.Application.Models;
using depensio.Application.UseCases.Settings.DTOs;
using depensio.Domain.Abstractions;
using depensio.Domain.Constants;
using depensio.Domain.Enums;
using depensio.Domain.Models;
using depensio.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace depensio.Application.Services;

public class BoutiqueSettingService(
    IDepensioDbContext _dbContext,
    IGenericRepository<BoutiqueSetting> _repository,
    IUnitOfWork _unitOfWork,
    IMemoryCache _cache,
    IUserContextService _userContextService,
    ISettingService _settingService
    ) : IBoutiqueSettingService
{

    public Task<Dictionary<string, object>> GetAllSettingsAsync(Guid boutiqueId)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetSettingAsync(Guid boutiqueId, string key)
    {

        var userId = _userContextService.GetUserId();
        //var cacheKey = $"boutique_setting_{boutiqueId}_{key}";

        //if (_cache.TryGetValue(cacheKey, out T? cachedValue))
        //    return cachedValue;

        //var setting = await _repository.FindAsync(b => b.BoutiqueId == BoutiqueId.Of(boutiqueId) && b.Key == key);

        var setting = await _dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(boutiqueId)
                        && b.UsersBoutiques.Any(ub => ub.UserId == userId))
            .SelectMany(b => b.BoutiqueSettings)
            .Where(s => s.Key == key)
            .Select(p => new SettingDTO
            {
               Key = p.Key,
               Value = p.Value
            })
            .FirstOrDefaultAsync();

        if (setting == null)
        {
            return _settingService.GetSetting(key);
        }         

        //var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(setting.Value);
        //_cache.Set(cacheKey, value, TimeSpan.FromMinutes(30));

        return setting.Value;
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
