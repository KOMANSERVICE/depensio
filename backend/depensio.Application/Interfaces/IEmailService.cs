using IDR.SendMail;

namespace depensio.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailModel emailModel);
}
