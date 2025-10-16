using AzureKeyVaultEmulator.Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
var keyVaultServiceName = "keyvault";

var keyVault = builder
    .AddAzureKeyVault(keyVaultServiceName)
    .RunAsEmulator(new KeyVaultEmulatorOptions { Persist = true });

var api = builder
    .AddProject<Projects.Conferenti_Api>("conferenti-api")
    .WithEndpoint("https", endpoint => endpoint.IsProxied = false)
    .WithReference(keyVault); // reference as normal


var frontend = builder
    .AddNpmApp("conferenti-frontend", "../../../../conferenti-web", "dev")
    .WithNpmPackageInstallation()
    .WaitFor(api)
    .WithReference(api)
    .WithHttpEndpoint(env: "3000") // Make sure Next.js uses the port assigned by Aspire
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
