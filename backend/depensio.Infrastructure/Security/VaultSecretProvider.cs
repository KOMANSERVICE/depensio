using depensio.Application.Interfaces;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;

namespace depensio.Infrastructure.Security;
public class VaultSecretProvider : ISecureSecretProvider
{
    private readonly IVaultClient _vaultClient;

    public VaultSecretProvider(string vaultUri, string roleId, string secretId)
    {
        var authMethod = new AppRoleAuthMethodInfo(roleId, secretId);
        var settings = new VaultClientSettings(vaultUri, authMethod);
        _vaultClient = new VaultClient(settings);
    }

    public async Task<string> GetSecretAsync(string key)
    {
        var secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            path: "depensio",
            mountPoint: "secret"
        );

        return secret.Data.Data[key]?.ToString() 
            ?? throw new KeyNotFoundException($"Key '{key}' not found in Vault path");
    }
}
