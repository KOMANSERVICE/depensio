using IDR.Library.BuildingBlocks.Exceptions.Handler;
using IDR.Library.BuildingBlocks.Security.Authentication;
using Microsoft.FeatureManagement.FeatureFilters;

namespace depensio.Api;


public static class DependencyInjection
{

    private static string MyAllowSpecificOrigins = "AllowOrigin";
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {

        var tempProvider = services.BuildServiceProvider();
        var vaultSecretProvider = tempProvider.GetRequiredService<ISecureSecretProvider>();

        var secretKey = configuration["JWT:Secret"]!;

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT Secret is not provided in configuration");
        }

        var JWT_Secret = vaultSecretProvider.GetSecretAsync(secretKey).Result;
        
        var JWT_ValidIssuer = configuration["JWT:ValidIssuer"];
        var JWT_ValidAudience = configuration["JWT:ValidAudience"];

        if(string.IsNullOrEmpty(JWT_ValidIssuer) || string.IsNullOrEmpty(JWT_ValidAudience))
        {
            throw new InvalidOperationException("JWT ValidIssuer or ValidAudience is not provided in configuration");
        }

        services.AddCarter();

        services.AddExceptionHandler<CustomExceptionHandler>();
        //services.AddHealthChecks()
        //.AddSqlServer(configuration.GetConnectionString("Database")!);

        //Add cors
        var Allow_origin = configuration["Allow:Origins"]!;
        if (string.IsNullOrEmpty(Allow_origin))
        {
            throw new InvalidOperationException("CORS origins are not provided in configuration");
        }
        var origin = vaultSecretProvider.GetSecretAsync(Allow_origin).Result;

        var jwtOptions = new JwtAuthOptions
        {
            Secret = JWT_Secret,
            Issuer = JWT_ValidIssuer,
            Audience = JWT_ValidAudience,
            AllowedOrigins = origin,
            CorsPolicyName = MyAllowSpecificOrigins,
        };


        services.AddJwtAuth(options =>
        {
            options.AllowedOrigins = jwtOptions.AllowedOrigins;
            options.CorsPolicyName = jwtOptions.CorsPolicyName;
        });

        services.AddAuthorizationBuilder().AddPolicy(MyAllowSpecificOrigins,
                policy => policy
                    .RequireRole("admin")
                    .RequireClaim("scope", "greetings_api"));

        services.AddEndpointsApiExplorer();

        services.AddAuthorization();
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = jwtOptions.SaveToken;
            options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;
            options.TokenValidationParameters = jwtOptions.CreateTokenValidationParameters();
        });

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
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
        app.UseJwtAuthCors();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            // Add the security scheme at the document level
            var securityScheme = new OpenApiSecurityScheme
            {
                Description = "Authorization oauth2",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = securityScheme;

            // Apply it as a requirement for all operations
            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            {
                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer")] = new List<string>()
                });
            }
        }
    }
}