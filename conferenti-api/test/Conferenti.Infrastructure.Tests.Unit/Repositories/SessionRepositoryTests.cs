using Conferenti.Domain.Sessions;
using Conferenti.Domain.Speakers;
using Conferenti.Infrastructure.Repositories;
using Shouldly;
using Xunit;

namespace Conferenti.Infrastructure.Tests.Unit.Repositories;

[CollectionDefinition("CosmosDb Collections")]
public class SessionRepositoryTests : IClassFixture<CosmosDbTestFixture>
{
    private readonly SessionRepository _sessionRepository;

    private SessionRepositoryTests(CosmosDbTestFixture fixture)
    {
        _sessionRepository = new SessionRepository(fixture.Container);
    }

    [Fact]
    public async Task GetSpeakers_Should_Return_Empty()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        var speakers = await _sessionRepository.GetSessions(new SessionParam(), cancellationToken);

        // Assert
        speakers.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetSpeakers_Should_Return_Speakers()
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
        var speakers = await _sessionRepository.GetSessions(new SessionParam(), cancellationToken);

        // Assert
        Assert.NotNull(speakers);
        Assert.NotEmpty(speakers);
    }

    [Fact]
    public async Task AddSpeakerAsync_Should_Return_Speaker()
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
