using depensio.Application.UseCases.ChatBots.DTOs;

namespace depensio.Application.UseCases.ChatBots.Commands;

public record UseChatBotCommand(ChatbotDTO Chatbot)
    : ICommand<UseChatBotResult>;

public record UseChatBotResult(string ResponseMessage);

public class UseChatBotValidator : AbstractValidator<UseChatBotCommand>
{
    public UseChatBotValidator()
    {
        RuleFor(x => x.Chatbot).NotNull().WithMessage("Le message ne peut pas être vide.");
        RuleFor(x => x.Chatbot.ChatInput).NotEmpty().WithMessage("Le message ne peut pas être vide.");
    }
}
