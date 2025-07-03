using depensio.Application.Interfaces;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace depensio.Infrastructure.Security;

public class KeyManagementService(ISecureSecretProvider secureSecret) : IKeyManagementService
{
    //private const string KeyFilePath = @"key.json";

    private static string GenerateEncryptionKey()
    {
        byte[] key = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        return Convert.ToBase64String(key);
    }

    private static string GenerateVersion(Dictionary<string, string> keys)
    {
        int versionNumber = 1;
        if(keys.Count > 0)
        {
            string lastVersion = keys.Keys.Max();
            if(int.TryParse(lastVersion.Substring(1), out int lastVersionNumber))
            {
                versionNumber = lastVersionNumber + 1;
            }
        }

        return $"v{versionNumber}";
    }

    private async Task<Dictionary<string, string>> LoadKeys()
    {
        var json = await secureSecret.GetSecretAsync("KEY");

        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, string>();
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    }

    public async Task RotateKey()
    {
        string newKey = GenerateEncryptionKey();
        var keys = await LoadKeys();
        keys[GenerateVersion(keys)] = newKey;
        string lastVersion = keys.Keys.Max();
        SaveKeys(keys);
    }

    public string GetLastKeyVerdion()
    {
        var keys = LoadKeys().Result;
        if (keys.Count == 0)
        {
            return null;
        }

        string lastVersion = keys.Keys.Max();
        return string.IsNullOrWhiteSpace(lastVersion) ? "v1" : lastVersion;
    }

    public string GetKey(string keyVersion)
    {
        var keys = LoadKeys().Result;
        return keys.ContainsKey(keyVersion) ? keys[keyVersion] : null;
    }

    private static void SaveKeys(Dictionary<string, string> keys)
    {
        string json = JsonSerializer.Serialize(keys, new JsonSerializerOptions { WriteIndented = true});
        //File.WriteAllText(KeyFilePath, json);
    }

}
