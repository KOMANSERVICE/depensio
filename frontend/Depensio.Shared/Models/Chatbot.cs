namespace depensio.Shared.Models;
public record ChatbotDTO
{
    public string ChatInput { get; set; }
    public string SessionId { get; set; }
}
public record UseChatBotResponse(string ResponseMessage);
public record UseChatBotRequest(ChatbotDTO Chatbot);

