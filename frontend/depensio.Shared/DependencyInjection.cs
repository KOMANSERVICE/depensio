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


        services.AddScoped<IAuthService, AuthService>();

        var uri = configuration["ApiSettings:Uri"]!;
        services.AddRefitClient<IAuthHttpService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri));

        return services;
    }

}
