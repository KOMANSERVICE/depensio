

using depensio.Application.Models;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;

namespace depensio.Application.Services;

public class TemplateRendererService : ITemplateRendererService
{
    private readonly string _templateDirectory;

    public TemplateRendererService(IWebHostEnvironment env)
    {
        _templateDirectory = Path.Combine(AppContext.BaseDirectory, "Templates");
    }

    public async Task<RenderedTemplate> RenderTemplateAsync(string templateName, Dictionary<string, string> values)
    {
        var filePath = Path.Combine(_templateDirectory, templateName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Template file '{templateName}' not found in '{_templateDirectory}'");

        var content = await File.ReadAllTextAsync(filePath);

        // 🔁 Remplace tous les {{clé}} par la valeur correspondante
        var renderedContent = Regex.Replace(content, @"\{\{(.*?)\}\}", match =>
        {
            var key = match.Groups[1].Value.Trim();
            return values.ContainsKey(key) ? values[key] : match.Value;
        });

        // 🏷️ Extraction du <title> et du <body>
        var titleMatch = Regex.Match(renderedContent, @"<title>(.*?)<\/title>", RegexOptions.IgnoreCase);
        
        var subject = titleMatch.Success ? titleMatch.Groups[1].Value.Trim() : "No Subject";
       

        return new RenderedTemplate
        {
            Subject = subject,
            Body = renderedContent
        };
    }
}
