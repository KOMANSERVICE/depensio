namespace depensio.Shared.Services;

public interface IChatbotService
{
    [Post("/chatbot")]
    Task<BaseResponse<UseChatBotResponse>> UseChatBotAsync(UseChatBotRequest request);
}
