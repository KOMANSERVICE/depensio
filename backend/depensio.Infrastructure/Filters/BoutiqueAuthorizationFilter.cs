
namespace depensio.Infrastructure.Filters;

public class BoutiqueAuthorizationFilter(
        IUserContextService _userContextService
    ) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var userId = _userContextService.GetUserId();

        // Récupérer boutiqueId depuis la route
        var routeValues = httpContext.Request.RouteValues;
        Guid boutiqueId = Guid.Empty;
        if (routeValues.TryGetValue("boutiqueId", out var boutiqueIdObj) && Guid.TryParse(boutiqueIdObj?.ToString(), out var parsedId))
        {
            boutiqueId = parsedId;
        }

        // Vérification
        if (string.IsNullOrEmpty(userId) || boutiqueId == Guid.Empty)
            return Results.Unauthorized();

        // Récupérer le service via DI
        var authorizationService = httpContext.RequestServices.GetRequiredService<AuthorizationService>();

        try
        {
            authorizationService.GetUserPermissionForBoutique(userId, boutiqueId);
        }
        catch (UnauthorizedException ex)
        {
            return Results.Forbid();
        }

        // Si tout est OK, continuer
        return await next(context);
    }
}
