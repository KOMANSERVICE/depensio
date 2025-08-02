namespace depensio.Domain.Models;

public class Setting : Entity<SettingId>
{
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
