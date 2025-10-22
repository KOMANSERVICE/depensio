using depensio.Api;
using depensio.Infrastructure;
using depensio.Infrastructure.Data.Extensions;
using Depensio.Application;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddEnvironmentVariables();


builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);


var app = builder.Build();

app.UseApiServices()
    .UseInfrastructureServices();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();

    // Utilisation de la nouvelle syntaxe MapOpenApi avec le bon chemin
    app.MapOpenApi("/swagger/v1/swagger.json");
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Depensio API v1");

        options.OAuthUsePkce();
        options.DisplayRequestDuration();
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);

        options.EnableDeepLinking();

    });
}

app.Run();
