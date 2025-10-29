using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conferenti.Api.Tests.Integration.Infrastructure;

namespace Conferenti.Api.Tests.Integration.Sessions;
public class GetSessionsTests : BaseFunctionalTest
{
    public GetSessionsTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetSessions_ShouldReturnOk()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        // Act
        var response = await HttpClient.GetAsync("/v1/sessions", cancellationToken);
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
