using depensio.Application.Models;
using depensio.Application.UseCases.Settings.DTOs;

namespace depensio.Application.UseCases.Settings.Queries.GetSettingByBoutique;
public record GetSettingByBoutiqueQuery(Guid BoutiqueId, string Key)
    : IQuery<GetSettingByBoutiqueResult>;
public record GetSettingByBoutiqueResult(SettingDTO Settings);
