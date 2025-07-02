using depensio.Services;
using depensio.Shared;
using depensio.Shared.Services;
using Microsoft.Extensions.Logging;

namespace depensio
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the depensio.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>()
                .AddHttpClientFactoryServices(builder.Configuration);

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
