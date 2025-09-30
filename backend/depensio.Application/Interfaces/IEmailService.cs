using IDR.SendMail;
using SendMail.Models;

namespace depensio.Application.Interfaces;

public interface IEmailService
{
    Task<RenderedTemplate> RenderHtmlTemplateAsync(string templateName, Dictionary<string, string> values);
    Task SendEmailAsync(EmailModel emailModel);
}
