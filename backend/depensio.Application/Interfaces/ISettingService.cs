namespace depensio.Application.Interfaces;

public interface ISettingService
{
    SettingDTO GetSetting(string key);
}