using depensio.Shared.Services;

namespace depensio.Services;

public class SecureStorageService : IStorageService
{
    public Task SetAsync(string key, string value)
    {
        Preferences.Set(key, value);
        return Task.CompletedTask;
    }
    public Task<string?> GetAsync(string key)
    {
        var value = Preferences.Get(key, null);
        return Task.FromResult<string?>(value);
    }
    public Task RemoveAsync(string key)
    {
        Preferences.Remove(key);
        return Task.CompletedTask;
    }
}
