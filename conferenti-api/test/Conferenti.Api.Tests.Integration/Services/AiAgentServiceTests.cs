using System.Net;
using System.Net.Http.Json;
using Conferenti.Api.Tests.Integration.Infrastructure;
using Conferenti.Domain.AiAgents;

namespace Conferenti.Api.Tests.Integration.Services;
public class AiAgentServiceTests : BaseFunctionalTest
{
    public AiAgentServiceTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task PostAiChat_ShouldReturnOk()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var request = new AiChatRequest
        {
            Message = "Hello AI Agent!",
            SessionId = "test-session-124"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/v1/ai/chat", request, cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var aiResponse = await response.Content.ReadFromJsonAsync<AiChatResponse>(cancellationToken: cancellationToken);
        Assert.NotNull(aiResponse);
        Assert.True(aiResponse!.Success);
        Assert.NotNull(aiResponse.Response);
        Assert.Equal("test-session-id", aiResponse.SessionId);
        Assert.True(aiResponse.Timestamp > DateTimeOffset.MinValue);
    }
}
