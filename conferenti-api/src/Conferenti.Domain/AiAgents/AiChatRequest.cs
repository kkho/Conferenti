namespace Conferenti.Domain.AiAgents;

public class AiChatRequest
{
    public string Message { get; set; } = string.Empty;

    public string SessionId { get; set; }
}
