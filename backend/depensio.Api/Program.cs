using depensio.Api;
using depensio.Infrastructure;
using depensio.Infrastructure.Data.Extensions;
using Depensio.Application;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddEnvironmentVariables();
// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();
