using depensio.Shared.Components.Toast;
using depensio.Shared.Services;
using IDR.Library.Blazor.Auths;
using IDR.Library.Blazor.Auths.Handlers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace depensio.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddHttpClientFactoryServices(this IServiceCollection services, IConfiguration configuration)
    {
        var uri = configuration["ApiSettings:Uri"]!;

        services.AddAuthServices(configuration, (options) =>
        {
            options.Uri = uri;
            options.Logout = "logout";
        });

        services.AddScoped<ToastService>();

        services.AddScoped<IFlowbiteService, FlowbiteService>();
        services.AddScoped<HeaderTabService>();
        

        services.AddRefitClient<IAuthHttpService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>();

        services.AddRefitClient<IChatbotService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>();        

        services.AddRefitClient<IBoutiqueService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>()
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IProductService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>()
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<ISaleService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>()
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IPurchaseService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>()
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IBoutiqueSettingService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>()
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IAuthUserService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>()
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IProfileService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>()
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IMenuService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>()
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddRefitClient<IDashboardServices>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri))
            .AddHttpMessageHandler<CookieHandler>()
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

        
        return services;
    }

}
