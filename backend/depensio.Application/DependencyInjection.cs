

namespace Depensio.Application;

public static class DependencyInjection
{
    public static IServiceCollection  AddApplicationServices
        (this IServiceCollection services, IConfiguration configuration)
    {

        services.AddValidationBehaviors(Assembly.GetExecutingAssembly());

        services.AddFeatureManagement();
        //services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());
        services.AddScoped<IBarcodeService, Ean13GeneratorService>();
        services.AddScoped<IBoutiqueSettingService, BoutiqueSettingService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<SettingOptionService>();
        services.AddScoped<AuthorizationService>();
        services.AddScoped<MenuService>();
        
        return services;
    }
}