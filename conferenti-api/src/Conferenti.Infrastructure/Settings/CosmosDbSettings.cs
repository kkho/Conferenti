namespace Conferenti.Infrastructure.Settings;

public record CosmosDbSettings(bool UseLocal, bool IntegrationTest, string AccountEndPoint, string Key);
