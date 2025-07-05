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
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "vault/shared/vault-depensio-env.json");
            var vaultSecrets = JsonSerializer.Deserialize<VaultSecretsConfig>(File.ReadAllText(jsonPath))!;


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
}