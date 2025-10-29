using Conferenti.Domain.Sessions;
using Conferenti.Infrastructure.Repositories;
using Shouldly;
using Xunit;

namespace Conferenti.Infrastructure.Tests.Unit.Repositories;

[CollectionDefinition("CosmosDb Collections")]
public class SessionRepositoryTests : IClassFixture<CosmosDbTestFixture>
{
    private readonly SessionRepository _sessionRepository;

    public SessionRepositoryTests(CosmosDbTestFixture fixture)
    {
        _sessionRepository = new SessionRepository(fixture.SessionContainer);
    }

    [Fact]
    public async Task GetSessions_Should_Return_Empty()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        var sessions = await _sessionRepository.GetSessions(new SessionParam(), cancellationToken);

        // Assert
        sessions.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetSessions_Should_Return_Speakers()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var session = new Session
        {
            Id = "session-doe-1",
            Title = "C#",
            Description = "A seasoned speaker on technology."
        };

        // Act
        await _sessionRepository.UpsertSessionsAsync([session], cancellationToken);
        var sessions = await _sessionRepository.GetSessions(new SessionParam(), cancellationToken);

        // Assert
        Assert.NotNull(sessions);
        Assert.NotEmpty(sessions);
    }

    [Fact]
    public async Task AddSessionsAsync_Should_Return_Speaker()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var session = new Session
        {
            Id = "session-doe-1",
            Title = "C#",
            Description = "A seasoned speaker on technology."
        };

        // Act
        var sessions = await _sessionRepository.UpsertSessionsAsync([session], cancellationToken);

        // Assert
        Assert.NotNull(session);
        sessions.First().Id.ShouldBe(session.Id);
    }
}
