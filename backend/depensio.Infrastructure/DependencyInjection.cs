using depensio.Application.Interfaces;
using depensio.Domain.Models;
using depensio.Infrastructure.Data;
using depensio.Infrastructure.Security;
using depensio.Infrastructure.Services;
using IDR.SendMail;
using IDR.SendMail.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


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


        var tempProvider = services.BuildServiceProvider();
        var vaultSecretProvider = tempProvider.GetRequiredService<ISecureSecretProvider>();
        var connectionString = vaultSecretProvider.GetSecretAsync(configuration.GetConnectionString("DataBase")!).Result;
        var fromMailIdPassword = vaultSecretProvider.GetSecretAsync(configuration["MailConfig:FromMailIdPassword"]!).Result;

        services.AddDbContext<DepensioDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
                //.EnableSensitiveDataLogging()
                //.EnableDetailedErrors()
                //.LogTo(Console.WriteLine, LogLevel.Information);
        });


        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<DepensioDbContext>()
            .AddDefaultTokenProviders();



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

        services.AddScoped<IKeyManagementService, KeyManagementService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddTransient<ISendMailService, SendMailService>();
        services.AddTransient<IEmailService, EmailService>();
        

        return services;        
    }
}
