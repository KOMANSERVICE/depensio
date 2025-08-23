using depensio.Shared.Pages.Auth.Models;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;
//using Microsoft.AspNetCore.Components.Authorization;

namespace depensio.Shared.Services;

public class AuthService : IAuthService
{
    private readonly IAuthHttpService _authHttpService;
    private readonly IStorageService _storage;
    private readonly CustomAuthStateProvider _authStateProvider;

    private const string TOKEN_KEY = "authToken";

    public AuthService(
        IAuthHttpService authHttpService,
        IStorageService storage,
        CustomAuthStateProvider authStateProvider)
    {
        _authHttpService = authHttpService;
        _storage = storage;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> SignInAsync(SignInRequest request)
    {
        var result = await _authHttpService.SignIn(request);
        if (!string.IsNullOrWhiteSpace(result.Token))
        {
            await _storage.SetAsync(TOKEN_KEY, result.Token);
            _authStateProvider.NotifyUserAuthentication(result.Token);
            return true;
        }
        else
        {
            return false;
        }        

    }

    public async Task LogoutAsync()
    {
        await _storage.RemoveAsync(TOKEN_KEY);
        _authStateProvider.NotifyUserLogout();
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _storage.GetAsync(TOKEN_KEY);
    }

    public async Task LoadTokenAsync()
    {
        await _authStateProvider.LoadTokenAsync();
    }

}