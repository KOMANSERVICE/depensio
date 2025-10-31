using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace depensio.Shared.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private readonly IStorageService _storage;
	public static string TOKEN_KEY = "authToken";
	private bool _isInitialized = false;
	private ClaimsPrincipal? _cachedUser;

	public CustomAuthStateProvider(IStorageService storage)
	{
		_storage = storage;
	}

	private ClaimsPrincipal Anonymous => new(new ClaimsIdentity());

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		// Pendant le rendu statique ou si pas encore initialisé, retourner un état anonyme
		if (!_isInitialized || _cachedUser == null)
		{
			return new AuthenticationState(Anonymous);
		}

		return new AuthenticationState(_cachedUser);
	}

	public async Task LoadTokenAsync()
	{
		try
		{
			var token = await _storage.GetAsync(TOKEN_KEY);
			if (!string.IsNullOrWhiteSpace(token))
			{
				var claims = ParseClaimsFromJwt(token);
				_cachedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
				NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
			}
		}
		catch
		{
			// En cas d'erreur (ex: rendu statique), on reste anonyme
			_cachedUser = Anonymous;
		}
		finally
		{
			_isInitialized = true;
		}
	}

	public void NotifyUserAuthentication(string token)
	{
		try
		{
			var claims = ParseClaimsFromJwt(token);
			_cachedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
			_isInitialized = true;
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
		}
		catch
		{
			_cachedUser = Anonymous;
			_isInitialized = true;
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
		}
	}

	public void NotifyUserLogout()
	{
		_cachedUser = Anonymous;
		_isInitialized = true;
		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
	}

	private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
	{
		try
		{
			var payload = jwt.Split('.')[1];
			var jsonBytes = DecodeBase64Url(payload);
			var claimsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)!;

			var claims = new List<Claim>();

            foreach (var kvp in claimsDict)
			{
				if (kvp.Key.Equals("role", StringComparison.OrdinalIgnoreCase) ||
					kvp.Key.Equals("roles", StringComparison.OrdinalIgnoreCase))
				{
					if (kvp.Value is JsonElement element)
					{
						if (element.ValueKind == JsonValueKind.Array)
						{
							foreach (var item in element.EnumerateArray())
							{
								claims.Add(new Claim(ClaimTypes.Role, item.GetString() ?? string.Empty));
							}
						}
						else
						{
							claims.Add(new Claim(ClaimTypes.Role, element.GetString() ?? string.Empty));
						}
					}
					continue;
				}

				var claimType = MapJwtClaimType(kvp.Key);
				claims.Add(new Claim(claimType, kvp.Value?.ToString() ?? string.Empty));
			}

            claims.Add(new Claim("token", jwt));
            return claims;
		}
		catch
		{
			// En cas d'erreur de parsing JWT, retourner des claims vides
			return Enumerable.Empty<Claim>();
		}
	}

	private static string MapJwtClaimType(string key)
	{
		return key switch
		{
			"name" or "unique_name" or "nameid" or "sub" => ClaimTypes.Name,
			"email" => ClaimTypes.Email,
			"given_name" => ClaimTypes.GivenName,
			"family_name" => ClaimTypes.Surname,
			"sid" => ClaimTypes.Sid,
			_ => key
		};
	}

	private static byte[] DecodeBase64Url(string base64Url)
	{
		var s = base64Url.Replace('-', '+').Replace('_', '/');
		switch (s.Length % 4)
		{
			case 2: s += "=="; break;
			case 3: s += "="; break;
		}
		return Convert.FromBase64String(s);
	}
}
