using depensio.Shared;
using depensio.Shared.Services;
using depensio.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the depensio.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>()
                .AddHttpClientFactoryServices(builder.Configuration);

await builder.Build().RunAsync();
