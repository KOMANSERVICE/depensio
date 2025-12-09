using ApexCharts;
using Blazored.LocalStorage;
using depensio.Shared;
using depensio.Shared.Pages.Dashboards.Models;
using depensio.Shared.Pages.Produits.Models;
using depensio.Shared.Services;
using depensio.Web.Client.Services;
using IDR.Library.Blazor.LocalStorages;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// ✅ CHARGER LA CONFIG DEPUIS L'ENDPOINT /api/config AVANT TOUT
try
{
    using var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

    var configResponse = await httpClient.GetFromJsonAsync<ConfigResponse>("/api/config");
    var apiUrl = configResponse?.ApiSettings?.Uri;

    Console.WriteLine($"🌐 API URL chargée depuis /api/config : {apiUrl}");

    // Remplacer la configuration par celle de l'API
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ApiSettings:Uri"] = apiUrl
    });
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erreur lors du chargement de /api/config : {ex.Message}");
    // Fallback sur la config par défaut
}

// Add device-specific services used by the depensio.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>()
                .AddScoped<IStorageService, SecureStorageService>()
                .AddBlazoredLocalStorage()
                .AddAuthorizationCore()
                .AddHttpClientFactoryServices(builder.Configuration);

builder.Services.AddApexCharts();
builder.Services.AddScoped<IGraphComponent<SaleSummary>, WebGraphComponentService>();
builder.Services.AddScoped<IGraphComponent<SaleDashboard>, SalesGraphComponentService>();


await builder.Build().RunAsync();
// ✅ AJOUTER CETTE CLASSE À LA FIN DU FICHIER
public class ConfigResponse
{
    public ApiSettingsConfig? ApiSettings { get; set; }
}

public class ApiSettingsConfig
{
    public string? Uri { get; set; }
}
