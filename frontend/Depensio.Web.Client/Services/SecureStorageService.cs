using Blazored.LocalStorage;
using depensio.Shared.Services;
using IDR.Library.Blazor.Enums;
using IDR.Library.Blazor.LocalStorages;

namespace depensio.Web.Client.Services;


public class SecureStorageService(ILocalStorageService _storage) : IStorageService
{
    public async Task SetAsync(string key, string value, StorageType storageType = StorageType.Local) => await _storage.SetItemAsync(key, value);

    public async Task<string?> GetAsync(string key)
    {
        return await _storage.GetItemAsync<string>(key);
    }

    public async Task RemoveAsync(string key) => await _storage.RemoveItemAsync(key);
}

