using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using Conferenti.Domain.Speakers;
using Conferenti.Infrastructure.Helpers;
using Conferenti.Infrastructure.Repositories;
using Conferenti.Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        }
        catch (HttpRequestException ex)
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

    private static async Task AddCosmosDbService(this IServiceCollection services, CosmosDbSettings? settings, CancellationToken token)
    {
        var clientOptions = new CosmosClientOptions()
        {
            UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Example: CamelCase property names
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Example: Ignore null properties
                // Add other System.Text.Json.JsonSerializerOptions as needed
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
            new ContainerProperties(Constants.SessionContainerId, "/sessionId"),
            throughput: 400,
            cancellationToken: token);

        services.AddKeyedSingleton<Container>("speakers", speakerContainer.Container);
        services.AddKeyedSingleton<Container>("sessions", sessionContainer.Container);
    }
}
