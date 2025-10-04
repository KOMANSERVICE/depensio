using depensio.Shared.Components.Toast;
using depensio.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace depensio.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddHttpClientFactoryServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<JwtAuthorizationHandler>();
        services.AddScoped<CustomAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
        services.AddScoped<ToastService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFlowbiteService, FlowbiteService>();
        services.AddScoped<HeaderTabService>();
        
        var uri = configuration["ApiSettings:Uri"]!;
        services.AddRefitClient<IAuthHttpService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri));

        services.AddRefitClient<IBoutiqueService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IProductService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<JwtAuthorizationHandler>();


        services.AddRefitClient<ISaleService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IPurchaseService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IBoutiqueSettingService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IAuthUserService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IProfileService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IMenuService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        return services;
    }

}
