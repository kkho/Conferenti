using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Conferenti.TestUtil;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Azure.Cosmos;
using Testcontainers.CosmosDb;
using Xunit;

namespace Conferenti.Infrastructure.Tests.Unit.Repositories;

public class CosmosDbTestFixture : IAsyncLifetime
{
    public CosmosDbContainer CosmosDbContainer { get; set; }
    public CosmosClient CosmosClient { get; set; }
    public Database Database { get; set; }
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

        var port = CosmosDbContainer.GetMappedPublicPort(8081);
        CosmosClient = new CosmosClient(CosmosDbContainer.GetConnectionString(), new CosmosClientOptions
        {
            HttpClientFactory = () =>
            {
                HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
                };

                return new HttpClient(httpMessageHandler);
            },
            ConnectionMode = ConnectionMode.Gateway, // Gateway mode is more stable with emulator
            LimitToEndpoint = true,
            RequestTimeout = TimeSpan.FromSeconds(60), // Increase timeout for emulator
            UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Example: CamelCase property names
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Example: Ignore null properties
                // Add other System.Text.Json.JsonSerializerOptions as needed
            }
        });

        // Retry database and container creation with exponential backoff
        await CosmosDbContainerExtensions.RetryAsync(async () =>
        {
            Database = await CosmosClient.CreateDatabaseIfNotExistsAsync("testdb");
            // Use /id (lowercase) to match the JSON property name
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
