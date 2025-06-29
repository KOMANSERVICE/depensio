//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;
//using VaultSharp;
//using VaultSharp.V1.AuthMethods.AppRole;

//namespace depensio.Infrastructure.Data;

//public class DesignTimeDepensioDbContextFactory : IDesignTimeDbContextFactory<DepensioDbContext>
//{
//    public DepensioDbContext CreateDbContext(string[] args)
//    {
//        try
//        {
//            // Chargement de la configuration
//            var config = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("appsettings.json", optional: true)
//                .AddEnvironmentVariables()
//                .Build();

//            var vaultUri = config["Vault:Uri"];
//            var roleId = config["Vault:RoleId"];
//            var secretId = config["Vault:SecretId"];

//            var authMethod = new AppRoleAuthMethodInfo(roleId, secretId);
//            var settings = new VaultClientSettings(vaultUri, authMethod);
//            var client = new VaultClient(settings);

//            var secret = client.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "depensio",
//                mountPoint: "secret"
//                ).Result;
//            var connectionString = secret.Data.Data["DataBase"].ToString();

//            var optionsBuilder = new DbContextOptionsBuilder<DepensioDbContext>();
//            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

//            return new DepensioDbContext(optionsBuilder.Options);

//        }
//        catch (Exception ex)
//        {
//            File.WriteAllText("ef-error.log", ex.ToString());
//            throw;
//        }
//    }
//}