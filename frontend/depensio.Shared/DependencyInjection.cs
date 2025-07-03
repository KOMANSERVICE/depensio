using depensio.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace depensio.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddHttpClientFactoryServices(this IServiceCollection services, IConfiguration configuration)
    {
        var uri = configuration["ApiSettings:Uri"]!;
        services.AddRefitClient<IAuthService>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri));

        return services;
    }

}
