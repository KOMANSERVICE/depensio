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

// ✅ Charger la config depuis l'API au lieu du fichier
using var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var configResponse = await httpClient.GetFromJsonAsync<Dictionary<string, Dictionary<string, string>>>("/api/config");
var apiUrl = configResponse?["ApiSettings"]?["Uri"];

// Ajouter la config manuellement
builder.Configuration.AddInMemoryCollection(new[]
{
    new KeyValuePair<string, string>("ApiSettings:Uri", apiUrl)
});

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
