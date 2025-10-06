using depensio.Shared.Pages.Dashboards.Models;
using depensio.Shared.Services;
using depensio.Web.Client.Components;
using Microsoft.AspNetCore.Components;

namespace depensio.Web.Client.Services;

public class SalesGraphComponentService : IGraphComponent<SaleDashboard>
{
    public RenderFragment Render(IEnumerable<SaleDashboard> Items) => builder =>
    {
        builder.OpenComponent(0, typeof(SalesGraphComponent));
        builder.AddAttribute(1, "SaleDashboards", Items);
        builder.CloseComponent();
    };
}
