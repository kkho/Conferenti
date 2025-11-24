using System.Net;
using System.Net.Http.Json;
using Conferenti.Application.Abstractions.Authentication;
using Conferenti.Domain.AiAgents;
using Conferenti.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace Conferenti.Infrastructure.Tests.Unit.Services;

public class AiAgentServiceTests : IDisposable
{
    private readonly Mock<IAuthenticationService> _authenticationServiceMock = new();
    private readonly Mock<ILogger<AiAgentService>> _loggerMock = new();
    private HttpClient _httpClient;

    private AiAgentService CreateService(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        _httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:8000")
        };

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_httpClient);

        _authenticationServiceMock.Setup(a => a.GetServiceTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("test-token");

        // Ensure HttpClient is disposed when AiAgentService is disposed
        return new AiAgentService(factoryMock.Object, _authenticationServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SendChatMessageAsync_ReturnsSuccess_WhenAgentRespondsOk()
    {
        // Arrange
        var expected = new AiChatResponse
        {
            Success = true,
            Response = "Hello, how can I assist you?",
            SessionId = "session-123",
            Timestamp = DateTimeOffset.UtcNow
        };

        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = JsonContent.Create(expected);

        var service = CreateService(response);

        // Act
        var result = await service.SendChatMessageAsync(new AiChatRequest()
        { Message = "Hi", SessionId = "session-1" });

        // Assert
        Assert.True(result.Success);
        Assert.Equal(expected.Response, result.Response);
        Assert.Equal(expected.SessionId, result.SessionId);
    }

    [Fact]
    public async Task SendChatMessageAsync_ReturnsError_WhenAgentReturnsNonSuccessStatus()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = JsonContent.Create(new { error = "fail" })
        };

        // Act
        var service = CreateService(response);
        var result = await service.SendChatMessageAsync(new AiChatRequest()
        { Message = "Hi", SessionId = "session-1" });

        // Assert
        Assert.False(result.Success);
        Assert.Contains("AI Agent returned status InternalServerError", result.Error);
    }

    [Fact]
    public async Task SendChatMessageAsync_ReturnsError_WhenAgentReturnsEmptyBody()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("")
        };

        var service = CreateService(response);

        // Act
        var result = await service.SendChatMessageAsync(new AiChatRequest { Message = "Hi", SessionId = "session-1" });

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Failed to communicate with AI Agent: The input does not", result.Error);
    }

    [Fact]
    public async Task SendChatMessageAsync_ReturnsError_WhenExceptionThrown()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        using var httpClient = new HttpClient(handlerMock.Object);
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var service = new AiAgentService(factoryMock.Object, _authenticationServiceMock.Object, _loggerMock.Object);

        // Act
        var result = await service.SendChatMessageAsync(new AiChatRequest { Message = "Hi", SessionId = "session-1" });

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Failed to communicate with AI Agent", result.Error);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
