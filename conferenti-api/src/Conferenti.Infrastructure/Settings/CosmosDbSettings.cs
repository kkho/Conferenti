namespace Conferenti.Infrastructure.Settings;

public record CosmosDbSettings(bool UseLocal, string AccountEndPoint, string Key);
