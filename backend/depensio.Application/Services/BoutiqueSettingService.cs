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

    public async Task<SettingDTO> GetSettingAsync(Guid boutiqueId, string key)
    {

        var userId = _userContextService.GetUserId();
        //var cacheKey = $"boutique_setting_{boutiqueId}_{key}";

        //if (_cache.TryGetValue(cacheKey, out T? cachedValue))
        //    return cachedValue;

        //var setting = await _repository.FindAsync(b => b.BoutiqueId == BoutiqueId.Of(boutiqueId) && b.Key == key);

        var settingBoutique = await _dbContext.Boutiques
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

        var settings = _settingService.GetSetting(key);

        if (settingBoutique == null)
        {
            return settings;
        }
        
        if(settingBoutique.Value.Equals(settings.Value))
        {
            return settingBoutique;
        }

        var resultBoutique = JsonSerializer.Deserialize<List<BoutiqueValue>>(settingBoutique.Value);
        var result = JsonSerializer.Deserialize<List<BoutiqueValue>>(settings.Value);
        //_cache.Set(cacheKey, value, TimeSpan.FromMinutes(30));
        var allSettings = resultBoutique
        .Union(result.Where(r => !resultBoutique.Any(rb => rb.Id == r.Id)))
        .ToList();

        settingBoutique.Value = JsonSerializer.Serialize(allSettings);

        return settingBoutique;
    }

    public Task<bool> HasSettingAsync(Guid boutiqueId, string key)
    {
        throw new NotImplementedException();
    }


    public async Task<Guid> UpsertAsync(SettingDTO setting)
    {
        var settingExist = _settingService.GetSetting(setting.Key);
        if(settingExist == null)
        {
            throw new BadRequestException($"Le paramètre '{setting.Key}' n'existe pas dans la configuration de l'application.");
        }

        var userId = _userContextService.GetUserId();
        // Rechercher le paramètre existant
        var existingBoutiqueSetting = await _dbContext.Boutiques
            .Where(b => b.Id == BoutiqueId.Of(setting.BoutiqueId)
                        && b.UsersBoutiques.Any(ub => ub.UserId == userId))
            .SelectMany(b => b.BoutiqueSettings)
            .Where(s => s.Key == setting.Key)
            .Select(p => new BoutiqueSetting
            {
                Id = p.Id,
                BoutiqueId = p.BoutiqueId,
                Key = p.Key,
                Value = p.Value
            })
            .FirstOrDefaultAsync();

        if (existingBoutiqueSetting != null)
        {
            // Mise à jour du paramètre existant
            existingBoutiqueSetting.Value = setting.Value;
            _repository.UpdateData(existingBoutiqueSetting);
        }
        else
        {
            // Création d'un nouveau paramètre
            var newSetting = new BoutiqueSetting
            {
                BoutiqueId = BoutiqueId.Of(setting.BoutiqueId),
                Key = setting.Key,
                Value = setting.Value
            };

            await _repository.AddDataAsync(newSetting);
            existingBoutiqueSetting = newSetting;
        }

        await _unitOfWork.SaveChangesDataAsync();

        return existingBoutiqueSetting.Id.Value;    

    }

}
