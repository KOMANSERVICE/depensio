using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous => new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new AuthenticationState(_anonymous); // Chargement à l'init plus tard
    }

    public void NotifyUserAuthentication(string token)
    {
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(Pad(payload)));
        var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        return claims.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private string Pad(string base64) => base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
}
