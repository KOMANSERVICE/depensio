using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace depensio.Infrastructure.Handlers;

/// <summary>
/// DelegatingHandler that propagates the JWT Authorization header from incoming requests
/// to outgoing HTTP requests to microservices.
/// </summary>
public class JwtPropagationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<JwtPropagationHandler> _logger;

    public JwtPropagationHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<JwtPropagationHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is not null)
        {
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.LogDebug("JWT token propagated to outgoing request: {RequestUri}", request.RequestUri);
            }
            else
            {
                _logger.LogDebug("No JWT token found in incoming request to propagate");
            }
        }
        else
        {
            _logger.LogWarning("HttpContext is null, cannot propagate JWT token");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
