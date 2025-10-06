using depensio.Shared.Pages.Produits.Models;
using depensio.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace depensio.Components;

public class MauiGraphComponentService : IGraphComponent<SaleSummary>
{
    public RenderFragment Render(IEnumerable<SaleSummary> Items) => builder =>
    {
        builder.OpenComponent(0, typeof(MauiGraphComponent));
        builder.AddAttribute(1, "SaleSummarys", Items);
        builder.CloseComponent();
    };
}