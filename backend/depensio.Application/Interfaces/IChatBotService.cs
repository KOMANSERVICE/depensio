using depensio.Application.Models;

namespace depensio.Application.Interfaces;

public interface IChatBotService
{
    Task<ReponseOutput> UseChatBotAsync(string sessionId, string chatInput);
}
