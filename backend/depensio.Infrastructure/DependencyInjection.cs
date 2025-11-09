using depensio.Application.Data;
using depensio.Infrastructure.ApiExterne.n8n;
using depensio.Infrastructure.Data;
using depensio.Infrastructure.Services;
using IDR.Library.BuildingBlocks;
using IDR.Library.BuildingBlocks.Contexts;
using IDR.Library.BuildingBlocks.Interceptors;
using IDR.Library.BuildingBlocks.Security;
using IDR.Library.BuildingBlocks.Security.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Refit;


namespace depensio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {

        var vaultUri = configuration["Vault:Uri"]!;
        var roleId = configuration["Vault:RoleId"]!;
        var secretId = configuration["Vault:SecretId"]!;
        var dataBase = configuration.GetConnectionString("DataBase")!;
        var mailPassword = configuration["MailConfig:FromMailIdPassword"]!;
        var fromMailId  = configuration["MailConfig:FromMailId"]!;
        var fromMailName = configuration["MailConfig:FromMailName"] ?? "Depensio";
        var host = configuration["MailConfig:Host"]!;
        var ports = configuration["MailConfig:Ports"]!;

        var N8Nuri = configuration["N8N:Uri"] ?? "";

        if (string.IsNullOrEmpty(host))
        {
            throw new InvalidOperationException("Mail Host is not provided in configuration");
        }

        if(string.IsNullOrEmpty(ports))
        {
            throw new InvalidOperationException("Mail Ports is not provided in configuration");
        }

        if (string.IsNullOrEmpty(dataBase))
        {
            throw new InvalidOperationException("Database connection string is not provided in configuration");
        }

        if (string.IsNullOrEmpty(fromMailId))
        {
            throw new InvalidOperationException("Mail FromMailId is not provided in configuration");
        }

        if (string.IsNullOrEmpty(mailPassword)) { 
            throw new InvalidOperationException("Mail FromMailIdPassword is not provided in configuration");
        }

        if (string.IsNullOrEmpty(host))
        {
            throw new InvalidOperationException("Mail Host is not provided in configuration");
        }

        if (string.IsNullOrEmpty(vaultUri) ||
            string.IsNullOrEmpty(roleId) ||
            string.IsNullOrEmpty(secretId))
        {
            throw new InvalidOperationException("Vault configuration is not provided in configuration");
        }


        services.AddSingleton<ISecureSecretProvider>(sp =>
            new VaultSecretProvider(
                configuration: configuration,
                vaultUri: vaultUri,
                roleId: roleId,
                secretId: secretId
            )
        );

        var tempProvider = services.BuildServiceProvider();
        var vaultSecretProvider = tempProvider.GetRequiredService<ISecureSecretProvider>();
        var connectionString = vaultSecretProvider.GetSecretAsync(dataBase).Result ?? "";
        var fromMailIdPassword = vaultSecretProvider.GetSecretAsync(mailPassword).Result ?? "";

        services.AddDbContext<DepensioDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.Configure<IdentityOptions>(options =>
        {
            // Default Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
        });

        services.AddEmailServices((options) =>
        {
            options.FromMailId = fromMailId;
            options.FromMailIdPassword = fromMailIdPassword;
            options.FromMailName = fromMailName;
            options.Host = host;
            options.Ports = int.Parse(ports);
            options.IsBodyHtml = true;
            options.EnableSsl = true;
        });

        services.AddScoped<IDepensioDbContext, DepensioDbContext>();

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<DepensioDbContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IEmailService, EmailService>();

        services.AddGenericRepositories<DepensioDbContext>();

        services.AddInterceptors();
        services.AddSecurities();
        services.AddContextMiddleware();

        services.AddScoped<IChatBotService, ChatBotService>();

        services.AddRefitClient<IN8NChatBotService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(N8Nuri));


        return services;
    }

    public static WebApplication UseInfrastructureServices(this WebApplication app)
    {
        app.UseContextMiddleware();
        return app;
    }
}
