using System.Net.Http.Headers;

namespace depensio.Shared.Services;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly IAuthService _authService;

    public JwtAuthorizationHandler(IAuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}