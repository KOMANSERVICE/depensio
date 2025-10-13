using depensio.Application.Interfaces;
using depensio.Application.Models;
using depensio.Infrastructure.ApiExterne.n8n;

namespace depensio.Infrastructure.Services;

public class ChatBotService(IN8NChatBotService n8nChatBotService)
    : IChatBotService
{

    public async Task<ReponseOutput> UseChatBotAsync(string sessionId, string chatInput)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            SessionId = sessionId,
            Action = "sendMessage",
            ChatInput = chatInput
        };
        var response = await n8nChatBotService.SendMessageAsync("5e56a263-3a40-44bd-bc9d-1cfb3bc2a87d", sendMessageRequest);
        return response;
    }

}
