
using BuildingBlocks.Exceptions;
using depensio.Application.Interfaces;
using IDR.SendMail;
using IDR.SendMail.Interfaces;
using SendMail.Models;

namespace depensio.Infrastructure.Services;

public class EmailService(ISendMailService _mailService) : IEmailService
{
    public async Task SendEmailAsync(EmailModel emailModel)
    {
        if (emailModel == null) throw new ArgumentNullException(nameof(emailModel));
        if (emailModel.ToMailIds == null || !emailModel.ToMailIds.Any())
            throw new BadRequestException("At least one recipient email address is required.", nameof(emailModel.ToMailIds));
        await _mailService.SendMail(emailModel);
    }

    public async Task<RenderedTemplate> RenderHtmlTemplateAsync(string templateName, Dictionary<string, string> values)
    {
        return await _mailService.RenderHtmlTemplateAsync(templateName, values);  
    }
}
