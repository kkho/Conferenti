namespace Conferenti.Infrastructure.Settings;

public record ServiceBusSettings(string ServiceBusName, string ConnectionString, string SessionQueueName, string SpeakerQueueName);
