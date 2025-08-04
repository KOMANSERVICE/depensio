using depensio.Domain.ValueObjects;

namespace depensio.Application.UseCases.Settings.DTOs;

public record BoutiqueSettingDTO
{
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
