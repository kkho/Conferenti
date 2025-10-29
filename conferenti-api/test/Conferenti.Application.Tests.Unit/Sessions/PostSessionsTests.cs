using Conferenti.Application.Sessions.PostSessions;
using Conferenti.Application.Speakers.PostSpeakers;
using Conferenti.Domain.Sessions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace Conferenti.Application.Tests.Unit.Sessions;

public class PostSessionsTests
{
    private readonly PostSessionCommandHandler _handler;
    private readonly ISessionRepository _sessionRepositoryMock;
    private readonly ILogger<PostSessionCommandHandler> _loggerMock;

    public PostSessionsTests()
    {
        _sessionRepositoryMock = Substitute.For<ISessionRepository>();
        _loggerMock = Substitute.For < ILogger<PostSessionCommandHandler>>();
        _handler = new PostSessionCommandHandler(_sessionRepositoryMock, _loggerMock);
    }

    [Fact]
    public async Task PostSessions_With_Data_Should_Return_Valid_Response()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var title = "Java";
        var description = "An expert in cloud computing.";

        var command = new PostSessionCommand([
            new Session
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Description = description,
            }
        ]);

        _sessionRepositoryMock.UpsertSessionsAsync(command.Sessions, cancellationToken)
            .Returns(command.Sessions);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        var speaker = result.Value.First();
        speaker.Id.ShouldNotBeNullOrEmpty();
        speaker.Id.ShouldNotBe(Guid.Empty.ToString());
        speaker.Title.ShouldBe(title);
        speaker.Title.ShouldBe(title);
    }
}
