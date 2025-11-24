using System.Net.Http.Json;
using Conferenti.Application.Abstractions.Authentication;
using Conferenti.Application.AiAgents;
using Conferenti.Domain.AiAgents;
using Microsoft.Extensions.Logging;

namespace Conferenti.Infrastructure.Services;
public class AiAgentService(
    IHttpClientFactory httpClientFactory,
    IAuthenticationService authenticationService,
    ILogger<AiAgentService> logger)
    : IAiAgentService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<AiChatResponse> SendChatMessageAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await authenticationService.GetServiceTokenAsync(cancellationToken);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/chat");
            requestMessage.Content = JsonContent.Create(request);

            requestMessage.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "AI Agent returned status {StatusCode}: {ReasonPhrase}",
                    response.StatusCode,
                    response.ReasonPhrase);

                return new AiChatResponse
                {
                    Success = false,
                    Error = $"AI Agent returned status {response.StatusCode}",
                    Timestamp = DateTimeOffset.UtcNow
                };
            }

            var result = await response.Content.ReadFromJsonAsync<AiChatResponse>(cancellationToken);
            return result ?? new AiChatResponse
            {
                Success = false,
                Error = "Empty response from AI Agent",
                Timestamp = DateTimeOffset.UtcNow
            };

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error communicating with AI Agent");
            return new AiChatResponse
            {
                Success = false,
                Error = $"Failed to communicate with AI Agent: {ex.Message}",
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }
}
