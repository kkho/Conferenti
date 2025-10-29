using System.Text.Json;
using System.Text.Json.Serialization;
using Conferenti.Domain.Sessions;
using Conferenti.TestUtil;
using Microsoft.Azure.Cosmos;
using Testcontainers.CosmosDb;
using Xunit;

namespace Conferenti.Infrastructure.Tests.Unit.Repositories;

public class CosmosDbTestFixture : IAsyncLifetime
{
    public CosmosDbContainer CosmosDbContainer { get; set; }
    public CosmosClient CosmosClient { get; set; }
    public Database Database { get; set; }

    public Container SessionContainer { get; set; }
    public Container Container { get; set; }

    public CosmosDbTestFixture()
    {
        CosmosDbContainer = new CosmosDbBuilder()
            .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await CosmosDbContainer.StartAsync();

        // Wait for Cosmos DB emulator to be fully ready
        await CosmosDbContainer.WaitForCosmosDbReadiness();

        // Get connection string with retry logic to ensure it's available
        string connectionString = null;
        await CosmosDbContainerExtensions.RetryAsync(async () =>
        {
            connectionString = CosmosDbContainer.GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not yet available");
            }
            await Task.CompletedTask;
        }, maxRetries: 10, delayMs: 1000);

        var port = CosmosDbContainer.GetMappedPublicPort(8081);
        CosmosClient = new CosmosClient(connectionString, new CosmosClientOptions
        {
            HttpClientFactory = () =>
            {
                HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
                };

                return new HttpClient(httpMessageHandler);
            },
            ConnectionMode = ConnectionMode.Gateway,
            LimitToEndpoint = true,
            RequestTimeout = TimeSpan.FromSeconds(60),
            UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            }
        });

        // Retry database and container creation with exponential backoff
        await CosmosDbContainerExtensions.RetryAsync(async () =>
        {
            Database = await CosmosClient.CreateDatabaseIfNotExistsAsync("testdb");
            SessionContainer = await Database.CreateContainerIfNotExistsAsync("sessions", "/id");
            Container = await Database.CreateContainerIfNotExistsAsync("testcontainer", "/id");
        }, maxRetries: 5, delayMs: 2000);
    }

    public void Dispose()
    {
        // Do "global" teardown here; Only called once.
    }

    public async Task DisposeAsync()
    {
        await CosmosDbContainer.DisposeAsync();
    }
}
