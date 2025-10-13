using depensio.Application.Models;
using Refit;

namespace depensio.Infrastructure.ApiExterne.n8n;

public interface IN8NChatBotService
{
    [Post("/webhook/{webhookId}/chat")]
    Task<ReponseOutput> SendMessageAsync(string webhookId,SendMessageRequest sendMessage);
}
