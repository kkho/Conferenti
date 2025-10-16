using Conferenti.Api.Tests.Integration.Infrastructure;

namespace Conferenti.Api.Tests.Integration.Speakers;
public class GetSpeakersTests : BaseFunctionalTest
{
    public GetSpeakersTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetSpeakers_ShouldReturnOk()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        // Act
        var response = await HttpClient.GetAsync("/v1/speakers", cancellationToken);
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
