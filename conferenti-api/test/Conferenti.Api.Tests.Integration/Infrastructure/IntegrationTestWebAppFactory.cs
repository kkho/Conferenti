using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Security.KeyVault.Secrets;
using Conferenti.Api.Settings;
using Conferenti.Application.Speakers.GetSpeakers;
using Conferenti.Application.Speakers.PostSpeakers;
using Conferenti.Domain.Speakers;
using Conferenti.Infrastructure.Repositories;
using Conferenti.TestUtil;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Testcontainers.CosmosDb;

namespace Conferenti.Api.Tests.Integration.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly CosmosDbContainer _cosmosDbContainer = new CosmosDbBuilder()
        .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
        .Build();

    public CosmosClient CosmosClient { get; set; }
    public Database Database { get; set; }
    public Container Container { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Override configuration to bypass Key Vault for integration tests
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                // Bypass Key Vault entirely for integration tests
                ["KeyVaultSettings:BypassKeyVault"] = "true",
                ["KeyVaultSettings:UseEmulator"] = "false",
                ["KeyVaultSettings:VaultEndPoint"] = "https://localhost:8081",
                // Use local telemetry
                ["TelemetrySettings:UseLocal"] = "true",
                ["CosmosDbSettings:IntegrationTest"] = "true"
            }!);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ILogger<GetSpeakerQueryHandler>>();
            services.RemoveAll<ILogger<PostSpeakerCommandHandler>>();

            // Remove any Key Vault services that might have been registered
            services.RemoveAll<SecretClient>();
            services.RemoveAll<CosmosClient>();
            services.RemoveAllKeyed<Container>("speakers");
            services.RemoveAllKeyed<Container>("sessions");
            services.RemoveAll<ISpeakerRepository>();


            services.AddOpenTelemetry()
                .WithTracing(tracingBuilder =>
                {
                    tracingBuilder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddConsoleExporter(); // Simulate traces
                })
                .WithMetrics(metricsBuilder =>
                {
                    metricsBuilder.AddAspNetCoreInstrumentation();
                    metricsBuilder.AddConsoleExporter(); // Simulate metrics
                });

            // Register CosmosClient pointing to the test container
            services.AddSingleton<CosmosClient>(provider => CosmosClient);
            services.AddKeyedSingleton<Container>("speakers", Container);
            services.AddScoped<ISpeakerRepository, SpeakerRepository>();
        });


    }

    public async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable($"CosmosDbSettings:IntegrationTest", "true");

        await _cosmosDbContainer.StartAsync();

        // Wait for Cosmos DB emulator to be fully ready
        await _cosmosDbContainer.WaitForCosmosDbReadiness();

        var port = _cosmosDbContainer.GetMappedPublicPort(8081);
        CosmosClient = new CosmosClient(_cosmosDbContainer.GetConnectionString(), new CosmosClientOptions
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

    public new async Task DisposeAsync()
    {
        await _cosmosDbContainer.DisposeAsync();
    }
}
