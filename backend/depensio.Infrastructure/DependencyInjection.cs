using depensio.Application.Interfaces;
using depensio.Infrastructure.Data;
using depensio.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;


namespace depensio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {

        var vaultUri = configuration["Vault:Uri"];
        var roleId = configuration["Vault:RoleId"];
        var secretId = configuration["Vault:SecretId"];

        services.AddSingleton<ISecureSecretProvider>(sp =>
            new VaultSecretProvider(
                vaultUri: vaultUri!,
                roleId: roleId!,
                secretId: secretId!
            )
        );


        // 2. Build ServiceProvider TEMPORARILY to resolve Vault
        var tempProvider = services.BuildServiceProvider();
        var vaultSecretProvider = tempProvider.GetRequiredService<ISecureSecretProvider>();
        var connectionString = vaultSecretProvider.GetSecretAsync(configuration.GetConnectionString("DataBase")!).Result;

        // 3. Configure EF Core with Vault connection string
        var serverVersion = new MySqlServerVersion(MySqlServerVersion.LatestSupportedServerVersion);

        services.AddDbContext<DepensioDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseMySql(connectionString, serverVersion);
                //.EnableSensitiveDataLogging()
                //.EnableDetailedErrors()
                //.LogTo(Console.WriteLine, LogLevel.Information);
        });

        return services;        
    }
}
