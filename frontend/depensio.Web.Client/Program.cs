using ApexCharts;
using Blazored.LocalStorage;
using depensio.Shared;
using depensio.Shared.Pages.Dashboards.Models;
using depensio.Shared.Pages.Produits.Models;
using depensio.Shared.Services;
using depensio.Web.Client.Services;
using IDR.Library.Blazor.LocalStorages;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

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
