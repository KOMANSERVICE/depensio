using depensio.Shared.Pages.Produits.Models;
using Microsoft.AspNetCore.Components;

namespace depensio.Shared.Services;

public interface IGraphComponent
{
    RenderFragment Render(IEnumerable<SaleSummary> SaleSummarys);
}
