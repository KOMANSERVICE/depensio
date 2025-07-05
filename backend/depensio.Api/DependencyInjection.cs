using BuildingBlocks.Exceptions.Handler;
using depensio.Application.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace depensio.Api;


public static class DependencyInjection
{

    private static string MyAllowSpecificOrigins = "AllowOrigin";
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {

        var tempProvider = services.BuildServiceProvider();
        var vaultSecretProvider = tempProvider.GetRequiredService<ISecureSecretProvider>();
        var JWT_Secret = vaultSecretProvider.GetSecretAsync(configuration["JWT:Secret"]!).Result;
        
        var JWT_ValidIssuer = configuration["JWT:ValidIssuer"];
        var JWT_ValidAudience = configuration["JWT:ValidAudience"];
        //var BackOfficeURL = configuration["BackOfficeURL"]!;

        services.AddCarter();

        services.AddExceptionHandler<CustomExceptionHandler>();
        //services.AddHealthChecks()
        //.AddSqlServer(configuration.GetConnectionString("Database")!);

        //Add cors
        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.WithOrigins(JWT_ValidIssuer)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
        });
        services.AddAuthorizationBuilder().AddPolicy(MyAllowSpecificOrigins,
                policy => policy
                    .RequireRole("admin")
                    .RequireClaim("scope", "greetings_api"));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Authorization oauth2",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        services.AddAuthorization();
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.RequireHttpsMetadata = false;
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = JWT_ValidAudience,
                    ValidIssuer = JWT_ValidIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_Secret))
                };
            });


        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.MapCarter();

       app.UseExceptionHandler(options => { });
        //app.UseHealthChecks("/health",
        //    new HealthCheckOptions
        //    {
        //        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        //    });

        app.UseHttpsRedirection();
        app.UseCors(MyAllowSpecificOrigins);
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
