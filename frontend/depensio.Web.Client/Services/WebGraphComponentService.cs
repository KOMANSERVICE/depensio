

using depensio.Shared.Pages.Produits.Models;
using depensio.Shared.Services;
using depensio.Web.Client.Components;
using Microsoft.AspNetCore.Components;

namespace depensio.Web.Client.Services;

public class WebGraphComponentService : IGraphComponent
{
    public RenderFragment Render(IEnumerable<SaleSummary> SaleSummarys) => builder =>
    {
        builder.OpenComponent(0, typeof(WebGraphComponent));
        builder.AddAttribute(1, "SaleSummarys", SaleSummarys);
        builder.CloseComponent();
    };
}