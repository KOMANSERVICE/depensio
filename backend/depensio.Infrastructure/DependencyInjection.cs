using BuildingBlocks.Security.Models;
using depensio.Application.Data;
using depensio.Application.Interfaces;
using depensio.Domain.Models;
using depensio.Infrastructure.ApiExterne.n8n;
using depensio.Infrastructure.Data;
using depensio.Infrastructure.Data.Interceptors;
using depensio.Infrastructure.Middlewares;
using depensio.Infrastructure.Repositories;
using depensio.Infrastructure.Services;
using IDR.SendMail;
using IDR.SendMail.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Reflection.Emit;
using System.Text.Json;


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
        var connectionString = vaultSecretProvider.GetSecretAsync(dataBase).Result;
        var fromMailIdPassword = vaultSecretProvider.GetSecretAsync(mailPassword).Result;

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

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<IDepensioDbContext, DepensioDbContext>();


        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<DepensioDbContext>()
            .AddDefaultTokenProviders();



        services.AddTransient<ISendMailService, SendMailService>();
        services.AddTransient<IEmailService, EmailService>();

        services.AddScoped<IKeyManagementService, KeyManagementService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<IChatBotService, ChatBotService>();

        services.AddRefitClient<IN8NChatBotService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(N8Nuri));

        //Pour fait fonctionner le middleware UserContextMiddleware
        services.AddHttpContextAccessor();

        return services;
    }

    public static WebApplication UseInfrastructureServices(this WebApplication app)
    {
        app.UseMiddleware<UserContextMiddleware>();
        return app;
    }
}
