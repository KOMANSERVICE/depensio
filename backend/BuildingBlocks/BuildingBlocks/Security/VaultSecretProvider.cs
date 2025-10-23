using BuildingBlocks.Security.Interfaces;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;

namespace BuildingBlocks.Security;
public class VaultSecretProvider : ISecureSecretProvider
{
    private readonly IVaultClient _vaultClient;

    private readonly IConfiguration _configuration;
    public VaultSecretProvider(IConfiguration configuration,string vaultUri, string roleId, string secretId)
    {
        var authMethod = new AppRoleAuthMethodInfo(roleId, secretId);
        var settings = new VaultClientSettings(vaultUri, authMethod);
        _vaultClient = new VaultClient(settings);
        _configuration = configuration;
    }

    public async Task<string> GetSecretAsync(string key)
    {
        var secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            path: _configuration["Vault:PathMountPoint"],
            mountPoint: _configuration["Vault:MountPoint"]
        );

         return secret.Data.Data[key]?.ToString() 
            ?? throw new KeyNotFoundException($"Key '{key}' not found in Vault path");
    }
}
