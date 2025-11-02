using ApexCharts;
using depensio.Shared;
using depensio.Shared.Pages.Dashboards.Models;
using depensio.Shared.Pages.Produits.Models;
using depensio.Shared.Services;
using depensio.Web.Client.Services;
using depensio.Web.Components;
using depensio.Web.Services;
using IDR.Library.Blazor.LocalStorages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddEnvironmentVariables();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the depensio.Shared project
builder.Services.AddSingleton<IFormFactor, WebFormFactor>()
                .AddScoped<IStorageService, WebSecureStorageService>()
                .AddHttpClientFactoryServices(builder.Configuration);
builder.Services.AddScoped<ProtectedLocalStorage>();


builder.Services.AddApexCharts();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IGraphComponent<SaleSummary>, WebGraphComponentService>();
builder.Services.AddScoped<IGraphComponent<SaleDashboard>, SalesGraphComponentService>();


// Fallback si pas de configuration JWT
builder.Services.AddAuthentication();


builder.Logging.SetMinimumLevel(LogLevel.Warning);
builder.Logging.AddFilter("Refit", LogLevel.Warning);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Middleware d'authentification
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(depensio.Shared._Imports).Assembly,
        typeof(depensio.Web.Client._Imports).Assembly);

app.Run();
