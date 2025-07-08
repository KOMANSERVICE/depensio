using depensio.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;

namespace depensio.Infrastructure.Data;

public class DesignTimeDepensioDbContextFactory : IDesignTimeDbContextFactory<DepensioDbContext>
{
    public DepensioDbContext CreateDbContext(string[] args)
    {
        try
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var vaultSecrets = LoadVaultSecrets(configuration);

            var authMethod = new AppRoleAuthMethodInfo(vaultSecrets.Vault__RoleId, vaultSecrets.Vault__SecretId);
            var settings = new VaultClientSettings(vaultSecrets.Vault__Uri, authMethod);
            var client = new VaultClient(settings);

            var secret = client.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "depensio",
                mountPoint: "secret"
                ).Result;
            var connectionString = secret.Data.Data["DataBase"].ToString();

            var optionsBuilder = new DbContextOptionsBuilder<DepensioDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new DepensioDbContext(optionsBuilder.Options);

        }
        catch (Exception ex)
        {
            File.WriteAllText("ef-error.log", ex.ToString());
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
    private VaultSecretsConfig LoadVaultSecrets(IConfiguration configuration)
    {
        var uri = configuration["Vault:Uri"]!;
        var roleId = configuration["Vault:RoleId"]!;
        var secretId = configuration["Vault:SecretId"]!;
        if (!string.IsNullOrWhiteSpace(uri) && !string.IsNullOrWhiteSpace(roleId) && !string.IsNullOrWhiteSpace(secretId))
        {
            return new VaultSecretsConfig
            {
                Vault__Uri = uri,
                Vault__RoleId = roleId,
                Vault__SecretId = secretId
            };
        }

        // Fallback: lecture du JSON en local
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "vault/shared/vault-depensio-env.json");
        jsonPath = jsonPath.Replace(@"backend\depensio.Api\", "");
        if (!File.Exists(jsonPath))
            throw new FileNotFoundException($"Vault secrets file not found at: {jsonPath}");

        var json = File.ReadAllText(jsonPath);
        return JsonSerializer.Deserialize<VaultSecretsConfig>(json)
            ?? throw new InvalidOperationException("Vault secrets JSON is invalid.");
    }

}
