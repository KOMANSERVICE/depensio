using depensio.Application.Data;
using depensio.Application.Interfaces;
using depensio.Domain.Models;
using depensio.Infrastructure.Data;
using depensio.Infrastructure.Data.Interceptors;
using depensio.Infrastructure.Middlewares;
using depensio.Infrastructure.Models;
using depensio.Infrastructure.Repositories;
using depensio.Infrastructure.Security;
using depensio.Infrastructure.Services;
using IDR.SendMail;
using IDR.SendMail.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Emit;
using System.Text.Json;


namespace depensio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var jsonPath = configuration["Vault:Path"]!;

        var vaultSecrets = JsonSerializer.Deserialize<VaultSecretsConfig>(
            File.ReadAllText(jsonPath)
        )!;

        services.AddSingleton<ISecureSecretProvider>(sp =>
            new VaultSecretProvider(
                vaultUri: vaultSecrets.Vault__Uri,
                roleId: vaultSecrets.Vault__RoleId,
                secretId: vaultSecrets.Vault__SecretId
            )
        );


        var tempProvider = services.BuildServiceProvider();
        var vaultSecretProvider = tempProvider.GetRequiredService<ISecureSecretProvider>();
        var connectionString = vaultSecretProvider.GetSecretAsync(configuration.GetConnectionString("DataBase")!).Result;
        var fromMailIdPassword = vaultSecretProvider.GetSecretAsync(configuration["MailConfig:FromMailIdPassword"]!).Result;

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
            options.FromMailId = configuration["MailConfig:FromMailId"];
            options.FromMailIdPassword = fromMailIdPassword;
            options.FromMailName = configuration["MailConfig:FromMailName"];
            options.Host = configuration["MailConfig:Host"];
            options.Ports = int.Parse(configuration["MailConfig:Ports"]!);
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
