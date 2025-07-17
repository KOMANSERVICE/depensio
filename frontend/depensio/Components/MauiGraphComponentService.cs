using depensio.Shared.Pages.Produits.Models;
using depensio.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace depensio.Components;

public class MauiGraphComponentService : IGraphComponent
{
    public RenderFragment Render(IEnumerable<SaleSummary> SaleSummarys) => builder =>
    {
        builder.OpenComponent(0, typeof(MauiGraphComponent));
        builder.AddAttribute(1, "SaleSummarys", SaleSummarys);
        builder.CloseComponent();
    };
}