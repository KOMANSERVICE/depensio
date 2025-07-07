using depensio.Shared.Services;

namespace depensio.Services;

public class SecureStorageService : IStorageService
{
    public async Task SetAsync(string key, string value) => await SecureStorage.Default.SetAsync(key, value);

    public async Task<string?> GetAsync(string key) => await SecureStorage.Default.GetAsync(key);

    public async Task RemoveAsync(string key) => SecureStorage.Default.Remove(key);
}
