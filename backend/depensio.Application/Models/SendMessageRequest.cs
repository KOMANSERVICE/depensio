namespace depensio.Application.Models;
public class SendMessageRequest
{
    public string SessionId { get; set; }
    public string Action { get; set; }
    public string ChatInput { get; set; }
}
