namespace Conferenti.Infrastructure.Settings;

public record Auth0Settings(string Authority, string Audience, string AiClientId, string AiClientSecret);
