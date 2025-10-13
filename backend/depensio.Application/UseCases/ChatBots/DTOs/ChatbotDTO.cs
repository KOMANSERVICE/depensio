namespace depensio.Application.UseCases.ChatBots.DTOs;

public record ChatbotDTO
{
    public string ChatInput { get; init; }
    public string SessionId { get; init; }
}
