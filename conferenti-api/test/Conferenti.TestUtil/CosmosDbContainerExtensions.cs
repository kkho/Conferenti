using Testcontainers.CosmosDb;

namespace Conferenti.TestUtil;

public static class CosmosDbContainerExtensions
{
    public static async Task WaitForCosmosDbReadiness(this CosmosDbContainer cosmosDbContainer)
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

                var response = await client.GetAsync($"https://localhost:{cosmosDbContainer.GetMappedPublicPort(8081)}/_explorer/index.html");

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

    public static async Task RetryAsync(Func<Task> action, int maxRetries, int delayMs)
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

}
