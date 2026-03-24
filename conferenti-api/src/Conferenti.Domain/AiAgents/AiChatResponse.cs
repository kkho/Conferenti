namespace Conferenti.Domain.AiAgents;

public class AiChatResponse
{
    public string Response { get; set; }

    public string SessionId { get; set; }

    public bool Success { get; set; }

    public string Error { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string Intent { get; set; }

    public string[] Topics { get; set; }
}
