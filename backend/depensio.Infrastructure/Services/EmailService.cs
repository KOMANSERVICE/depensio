
using BuildingBlocks.Exceptions;
using depensio.Application.Interfaces;
using IDR.SendMail;
using IDR.SendMail.Interfaces;

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
    //private readonly IConfiguration _configuration;
    //public EmailService(IConfiguration configuration)
    //{
    //    _configuration = configuration;
    //}
    //public string GetEmailTemplate(string templateName)
    //{
    //    var templatePath = Path.Combine(_configuration["EmailTemplatesPath"], $"{templateName}.html");
    //    if (!File.Exists(templatePath))
    //    {
    //        throw new FileNotFoundException($"Email template '{templateName}' not found at path '{templatePath}'.");
    //    }
    //    return File.ReadAllText(templatePath);
    //}
}
