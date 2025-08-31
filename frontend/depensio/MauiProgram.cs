using ApexCharts;
using depensio.Components;
using depensio.Services;
using depensio.Shared;
using depensio.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace depensio
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            // Ajout de la config appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the depensio.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>()
                .AddScoped<IStorageService, SecureStorageService>()
                .AddHttpClientFactoryServices(builder.Configuration);

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped<IGraphComponent, MauiGraphComponentService>();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Exemple si tu utilises ApexCharts aussi dans Hybrid
            builder.Services.AddApexCharts();

            // Auth
          
            builder.Services.AddAuthorizationCore();


            return builder.Build();
        }
    }
}
