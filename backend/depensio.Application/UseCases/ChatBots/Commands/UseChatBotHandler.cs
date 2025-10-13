

namespace depensio.Application.UseCases.ChatBots.Commands;

public class UseChatBotHandler(
        IChatBotService chatBotService,
        IEmailService emailService,
        IConfiguration configuration
    )
    : ICommandHandler<UseChatBotCommand, UseChatBotResult>
{
    public async Task<UseChatBotResult> Handle(UseChatBotCommand request, CancellationToken cancellationToken)
    {

        var result = await chatBotService.UseChatBotAsync(request.Chatbot.SessionId, request.Chatbot.ChatInput);

        var output = result.Output;
        if (!string.IsNullOrEmpty(output.Email) && !string.IsNullOrEmpty(output.Nom))
        {
            var emails = configuration["Email:Support"]!.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var emailModel = new EmailModel
            {
                ToMailIds = emails.ToList(),
                Suject = "Réponse du ChatBot",
                Body = $"""
                <p>Bonjour,</p>
                <p>Question sans reponse par le chatbot.</P>
                <p>Nom: {output.Nom}</p>
                <p>Téléphone: {output.Telephone}</p>
                <p>Email: {output.Email}</p>  
                <p>Question: {output.Question}</p> 
                """
            };

            if(emails.Count() > 0)
            {
                await emailService.SendEmailAsync(emailModel);
            }
            
        }

        return new UseChatBotResult(result.Output.Reponse);
    }
}
