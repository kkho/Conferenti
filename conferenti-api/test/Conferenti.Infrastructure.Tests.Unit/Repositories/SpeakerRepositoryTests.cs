using Conferenti.Domain.Speakers;
using Conferenti.Infrastructure.Repositories;
using Shouldly;
using Xunit;

namespace Conferenti.Infrastructure.Tests.Unit.Repositories;

[CollectionDefinition("CosmosDb Collections")]
public class SpeakerRepositoryTests : IClassFixture<CosmosDbTestFixture>
{
    private readonly SpeakerRepository _speakerRepository;

    public SpeakerRepositoryTests(CosmosDbTestFixture fixture)
    {
        _speakerRepository = new SpeakerRepository(fixture.Container);
    }

    [Fact]
    public async Task GetSpeakers_Should_Return_Empty()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        var speakers = await _speakerRepository.GetSpeakers(cancellationToken);

        // Assert
        speakers.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetSpeakers_Should_Return_Speakers()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var speaker = new Speaker
        {
            Id = "john-doe-1",
            Name = "John Doe",
            Bio = "A seasoned speaker on technology.",
            Company = "Tech Corp",
            Position = "Senior Developer",
            PhotoUrl = "http://someurl.com",
            SpeakerSessions = []
        };

        // Act
        await _speakerRepository.UpsertSpeakersAsync([speaker], cancellationToken);
        var speakers = await _speakerRepository.GetSpeakers(cancellationToken);

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
        var speaker = new Speaker
        {
            Id = "john-doe-1",
            Name = "John Doe",
            Bio = "A seasoned speaker on technology.",
            Company = "Tech Corp",
            Position = "Senior Developer",
            PhotoUrl = "http://someurl.com",
            SpeakerSessions = []
        };

        // Act
        var speakers = await _speakerRepository.UpsertSpeakersAsync([speaker], cancellationToken);

        // Assert
        Assert.NotNull(speakers);
        speakers.First().Id.ShouldBe(speaker.Id);
    }
}
