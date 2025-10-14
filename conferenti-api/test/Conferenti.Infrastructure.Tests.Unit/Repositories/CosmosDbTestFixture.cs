using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        await WaitForCosmosDbReadiness();

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
            RequestTimeout = TimeSpan.FromSeconds(60) // Increase timeout for emulator
        });

        // Retry database and container creation with exponential backoff
        await RetryAsync(async () =>
        {
            Database = await CosmosClient.CreateDatabaseIfNotExistsAsync("testdb");
            // Use /id (lowercase) to match the JSON property name
            Container = await Database.CreateContainerIfNotExistsAsync("testcontainer", "/id");
        }, maxRetries: 5, delayMs: 2000);
    }

    private async Task WaitForCosmosDbReadiness()
    {
        var maxWaitTime = TimeSpan.FromMinutes(2);
        var checkInterval = TimeSpan.FromSeconds(5);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        while (stopwatch.Elapsed < maxWaitTime)
        {
            try
            {
                using var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
                };

                using var client = new HttpClient(handler);

                var response = await client.GetAsync($"https://localhost:{CosmosDbContainer.GetMappedPublicPort(8081)}/_explorer/index.html");

                if (response.IsSuccessStatusCode)
                {
                    // Additional wait to ensure service is fully initialized
                    await Task.Delay(5000);
                    return;
                }
            }
            catch
            {
                // Container not ready yet, continue waiting
            }

            await Task.Delay(checkInterval);
        }

        throw new TimeoutException("Cosmos DB emulator did not become ready within the expected time.");
    }

    private async Task RetryAsync(Func<Task> action, int maxRetries, int delayMs)
    {
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                Console.WriteLine($"Retry {i + 1}/{maxRetries} failed: {ex.Message}");
                await Task.Delay(delayMs * (i + 1)); // Exponential backoff
            }
        }
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
