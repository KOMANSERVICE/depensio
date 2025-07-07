using depensio.Shared.Services;

namespace depensio.Web.Client.Services;


public class SecureStorageService() : IStorageService
{
    //    public async Task SetAsync(string key, string value) => await _storage.SetAsync(key, value);

    //    public async Task<string?> GetAsync(string key)
    //    {
    //        var stored = await _storage.GetAsync<string>(key);
    //        return stored.Success ? stored.Value : null;
    //    }

    //    public async Task RemoveAsync(string key) => await _storage.DeleteAsync(key);
    public Task<string?> GetAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync(string key, string value)
    {
        throw new NotImplementedException();
    }
}

