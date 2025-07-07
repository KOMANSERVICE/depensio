using depensio.Shared;
using depensio.Shared.Services;
using depensio.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the depensio.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>()
                .AddSingleton<IStorageService, SecureStorageService>()
                .AddHttpClientFactoryServices(builder.Configuration);

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
