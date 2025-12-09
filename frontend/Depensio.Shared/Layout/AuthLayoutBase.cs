using depensio.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace depensio.Shared.Layout;

public class AuthLayoutBase : LayoutComponentBase
{
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IAuthService AuthService { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;

    protected bool IsLoaded { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        // Pas de logique d'auth ici pour éviter les problèmes de rendu statique
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("depensio.initialized");

            try
            {
                // Charger le token et notifier l'état de manière asynchrone
                await AuthService.LoadTokenAsync();
                IsLoaded = true;
                StateHasChanged();
            }
            catch
            {
                // En cas d'erreur, on continue avec l'état anonyme
                IsLoaded = true;
            }
        }
    }
}
