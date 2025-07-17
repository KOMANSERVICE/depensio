using ApexCharts;
using depensio.Shared;
using depensio.Shared.Services;
using depensio.Web.Components;
using depensio.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the depensio.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>()
                .AddScoped<IStorageService, SecureStorageService>()
                .AddHttpClientFactoryServices(builder.Configuration);
builder.Services.AddScoped<ProtectedLocalStorage>();

builder.Services.AddApexCharts();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IGraphComponent, WebGraphComponentService>();

var JWTValidIssuer = builder.Configuration["JWT:ValidIssuer"];
var JWTValidAudience = builder.Configuration["JWT:ValidAudience"];
var JWTSecret = builder.Configuration["JWT:Secret"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = JWTValidIssuer,
            ValidAudience = JWTValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JWTSecret!))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication(); // ?? obligatoire
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(depensio.Shared._Imports).Assembly,
        typeof(depensio.Web.Client._Imports).Assembly);

app.Run();
