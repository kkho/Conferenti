using AzureKeyVaultEmulator.Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
var keyVaultServiceName = "keyvault";

var keyVault = builder.AddAzureKeyVaultEmulator(keyVaultServiceName);

var serviceBus = builder
    .AddAzureServiceBus("conferentiservicebus")
    .RunAsEmulator(c => c
        .WithLifetime(ContainerLifetime.Persistent));

serviceBus.AddServiceBusQueue("speaker-queue");
serviceBus.AddServiceBusQueue("session-queue");

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
