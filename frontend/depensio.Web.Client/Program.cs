using ApexCharts;
using Blazored.LocalStorage;
using depensio.Shared;
using depensio.Shared.Services;
using depensio.Web.Client.Services;
using depensio.Web.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the depensio.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>()
                .AddScoped<IStorageService, SecureStorageService>()
                .AddHttpClientFactoryServices(builder.Configuration);

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddApexCharts();
builder.Services.AddScoped<IGraphComponent, WebGraphComponentService>();

await builder.Build().RunAsync();
