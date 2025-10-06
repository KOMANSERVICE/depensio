using depensio.Shared.Pages.Dashboards.Models;
using Microsoft.AspNetCore.Components;

namespace depensio.Shared.Services;

public interface IGraphComponent<T>
{
    RenderFragment Render(IEnumerable<T> Items);
}

