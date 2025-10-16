namespace Conferenti.Api.Tests.Integration.Infrastructure;

public class BaseFunctionalTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;
    protected readonly IntegrationTestWebAppFactory _factory;

    public BaseFunctionalTest(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        HttpClient = _factory.CreateClient();
    }
}
