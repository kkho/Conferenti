using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Conferenti.Domain.Sessions;
using Conferenti.Domain.Speakers;
using Conferenti.Infrastructure.Helpers;
using Conferenti.Infrastructure.Repositories;
using Conferenti.Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;

namespace Conferenti.Infrastructure;

public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var cosmosDbSettings = configuration.GetSection("CosmosDbSettings").Get<CosmosDbSettings>();

        try
        {
            await services.AddCosmosDbService(cosmosDbSettings, CancellationToken.None);
            services.AddScoped<ISpeakerRepository, SpeakerRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
        }
        catch (Exception ex)
        {
            if (cosmosDbSettings is { IntegrationTest: false })
            {
                throw new Exception("Failed to connect to Cosmos DB. Please ensure that the Cosmos DB emulator is running if using local settings.", ex);
            }
        }
        services.AddHttpClient();
        services.AddMemoryCache();
        services.AddResponseCompression();
        return services;
    }

    public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusSettings = configuration.GetSection("ServiceBus").Get<ServiceBusSettings>();

        if (serviceBusSettings is null)
        {
            throw new ArgumentException($"{nameof(serviceBusSettings)}: ServiceBus settings are not configured properly");
        }

        services.AddSingleton<ServiceBusClient>(sp => new ServiceBusClient(serviceBusSettings.ConnectionString));

        services.AddKeyedSingleton<ServiceBusSender>("session", (sp, key) =>
        {
            var client = sp.GetRequiredService<ServiceBusClient>();
            return client.CreateSender(serviceBusSettings.SessionQueueName);
        });

        services.AddKeyedSingleton<ServiceBusSender>("speaker", (sp, key) =>
        {
            var client = sp.GetRequiredService<ServiceBusClient>();
            return client.CreateSender(serviceBusSettings.SpeakerQueueName);
        });

        return services;
    }

    private static async Task AddCosmosDbService(this IServiceCollection services, CosmosDbSettings? settings, CancellationToken token)
    {
        var clientOptions = new CosmosClientOptions()
        {
            UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Example: CamelCase property names
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Example: Ignore null properties
                Converters = { new JsonStringEnumConverter() }
            }
        };

        if (settings is  { UseLocal : true})
        {
            services.AddSingleton<CosmosClient>(provider => new CosmosClient(settings.AccountEndPoint, settings.Key, clientOptions));
        }
        else
        {
            services.AddSingleton<CosmosClient>(provider => new CosmosClient(settings?.AccountEndPoint, new DefaultAzureCredential(), clientOptions));
        }

        var cosmosClient = services.BuildServiceProvider().GetService<CosmosClient>();
        var database = await cosmosClient!.CreateDatabaseIfNotExistsAsync(Constants.CosmosDatabaseId, cancellationToken: token);

        var speakerContainer = await database.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties(Constants.SpeakerContainerId, "/id"),
            throughput: 400,
            cancellationToken: token);

        // Recommended partition key for Session: /sessionId (or /speakerId if you query by speaker)
        var sessionContainer = await database.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties(Constants.SessionContainerId, "/id"),
            throughput: 400,
            cancellationToken: token);

        services.AddKeyedSingleton<Container>("speakers", speakerContainer.Container);
        services.AddKeyedSingleton<Container>("sessions", sessionContainer.Container);
    }
}
