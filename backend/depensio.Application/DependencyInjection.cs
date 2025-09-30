using BuildingBlocks.Behaviors;
using depensio.Application.Services;
using depensio.Application.UseCases.Auth.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using System.Reflection;

namespace Depensio.Application;

public static class DependencyInjection
{
    public static IServiceCollection  AddApplicationServices
        (this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddFeatureManagement();
        //services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());
        services.AddScoped<IBarcodeService, Ean13GeneratorService>();
        services.AddScoped<IBoutiqueSettingService, BoutiqueSettingService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<SettingOptionService>();
        
        return services;
    }
}