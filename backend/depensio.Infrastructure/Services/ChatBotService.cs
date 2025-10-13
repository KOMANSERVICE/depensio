using depensio.Application.Interfaces;
using depensio.Application.Models;
using depensio.Infrastructure.ApiExterne.n8n;
using Microsoft.Extensions.Configuration;

namespace depensio.Infrastructure.Services;

public class ChatBotService(
    IN8NChatBotService n8nChatBotService,
    IConfiguration configuration)
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
        var response = await n8nChatBotService.SendMessageAsync(configuration["WebhookId"]!, sendMessageRequest);
        return response;
    }

}
