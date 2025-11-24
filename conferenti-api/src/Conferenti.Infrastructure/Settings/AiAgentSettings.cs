namespace Conferenti.Infrastructure.Settings;

#pragma warning disable CA1054
public record AiAgentSettings(string BaseUrl, int Timeout = 60);
#pragma warning restore CA1054
