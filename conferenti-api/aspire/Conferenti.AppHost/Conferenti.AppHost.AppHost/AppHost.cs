using System.Runtime.InteropServices;
using Aspire.Hosting;
using AzureKeyVaultEmulator.Aspire.Hosting;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
var keyVaultServiceName = "keyvault";

var localOaiKey = "ollama";

var auth0Domain = builder.Configuration.GetValue<string>("Auth0:Domain");
var auth0Audience = builder.Configuration.GetValue<string>("Auth0:Audience");
var aiClientId = builder.Configuration.GetValue<string>("Auth0:AiClientId");
var aiClientSecret = builder.Configuration.GetValue<string>("Auth0:AiClientSecret");
var cosmosDbEndPoint = builder.Configuration.GetValue<string>("CosmosDb:AccountEndPoint");
var cosmosDbKey = builder.Configuration.GetValue<string>("CosmosDb:Key");

var dockerHost = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
    ? "172.17.0.1"  // Linux Docker bridge IP
    : "host.docker.internal";  // Windows/Mac

// Replace localhost with host.docker.internal for Docker containers
var cosmosDbEndpointForDocker = string.IsNullOrEmpty(cosmosDbEndPoint)
    ? $"https://{dockerHost}:8081/"
    : cosmosDbEndPoint.Replace("localhost", dockerHost).Replace("127.0.0.1", dockerHost);

var keyVault = builder.AddAzureKeyVaultEmulator(keyVaultServiceName);

var serviceBus = builder
    .AddAzureServiceBus("conferentiservicebus")
    .RunAsEmulator(c => c
        .WithLifetime(ContainerLifetime.Persistent));

serviceBus.AddServiceBusQueue("speaker-queue");
serviceBus.AddServiceBusQueue("session-queue");


var aiAgent = builder.AddDockerfile("conferenti-ai-agent", "../../../../conferenti-ai-agent", "Dockerfile")
    .WithEndpoint("https", endpoint =>
    {
        endpoint.IsProxied = false;
        endpoint.TargetPort = 8000;
    })
    .WithEnvironment("SIMULATOR_MODE", "generate")
    .WithEnvironment("PROJECT_ENDPOINT", $"http://{dockerHost}:11434")
    .WithEnvironment("LOCAL", "true")
    .WithEnvironment("AUTH0_DOMAIN", auth0Domain)
    .WithEnvironment("AUTH0_AUDIENCE", auth0Audience)
    .WithEnvironment("AUTH0_CLIENT_ID", aiClientId)
    .WithEnvironment("AUTH0_CLIENT_SECRET", aiClientSecret)
    .WithEnvironment("AI_API_KEY", localOaiKey)
    .WithEnvironment("COSMOSDB_ENDPOINT", cosmosDbEndpointForDocker)
    .WithEnvironment("COSMOSDB_KEY", cosmosDbKey)
    .WithContainerRuntimeArgs("--add-host=host.docker.internal:host-gateway")
    .ExcludeFromManifest();

var api = builder
    .AddProject<Projects.Conferenti_Api>("conferenti-api")
    .WithEndpoint("https", endpoint => endpoint.IsProxied = false)
    .WithReference(keyVault); // reference as normal

var frontend = builder
    .AddNpmApp("conferenti-frontend", "../../../../conferenti-web", "dev")
    .WithNpmPackageInstallation()
    .WaitFor(api)
    .WithReference(api)
    .WithHttpEndpoint(env: "3000", name: "conferenti-frontend") // Make sure Next.js uses the port assigned by Aspire
    .WithExternalHttpEndpoints();

var adminApi = builder.AddGolangApp("conferenti-admin-api", "../../../../conferenti-admin-api/cmd/admin-api")
    .WithHttpEndpoint(env: "PORT", name: "admin-api")
    .WithExternalHttpEndpoints();

var adminFrontend = builder.AddProject<Projects.Conferenti_Admin_WebApp>("conferenti-admin-web")
    .WaitFor(adminApi)
    .WithReference(adminApi)
    .WithHttpEndpoint(env: "5001", name: "admin-frontend")
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
