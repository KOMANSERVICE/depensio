using depensio.Shared.Pages.Dashboards.Models;
using depensio.Shared.Pages.Produits.Models;
using depensio.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace depensio.Components;

public class SalesMauiGraphComponentService : IGraphComponent<SaleDashboard>
{
    public RenderFragment Render(IEnumerable<SaleDashboard> Items) => builder =>
    {
        builder.OpenComponent(0, typeof(SalesMauiGraphComponent));
        builder.AddAttribute(1, "SaleSummarys", Items);
        builder.CloseComponent();
    };
}