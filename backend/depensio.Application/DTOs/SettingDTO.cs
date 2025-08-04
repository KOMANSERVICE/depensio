namespace depensio.Application.DTOs;

public record SettingDTO
{
    public Guid BoutiqueId { get; set; } = Guid.Empty;
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

}
