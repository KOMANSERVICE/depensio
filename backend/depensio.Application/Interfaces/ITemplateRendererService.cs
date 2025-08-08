using depensio.Application.Models;

namespace depensio.Application.Interfaces;

public interface ITemplateRendererService
{
    Task<RenderedTemplate> RenderTemplateAsync(string templateName, Dictionary<string, string> values);
}
