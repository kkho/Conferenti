using Conferenti.Application.Sessions.GetSessions;
using Conferenti.Domain.Sessions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace Conferenti.Application.Tests.Unit.Sessions;

public class GetSessionsTests
{
    private readonly ISessionRepository _sessionRepositoryMock;
    private readonly ILogger<GetSessionQueryHandler> _loggerMock;
    private readonly GetSessionQueryHandler _handler;

    public GetSessionsTests()
    {
        _sessionRepositoryMock = Substitute.For<ISessionRepository>();
        _loggerMock = Substitute.For<ILogger<GetSessionQueryHandler>>();
        _handler = new GetSessionQueryHandler(_sessionRepositoryMock, _loggerMock);
    }

    [Fact]
    public async Task? GetSessions_ShouldReturn_Success()
    {
        // Arrange
        var sessionParam = new SessionParam();
        var cancellationToken = CancellationToken.None;
        _sessionRepositoryMock.GetSessions(sessionParam, cancellationToken).Returns([]);

        // Act
        var result = await _handler.Handle(new GetSessionsQuery(sessionParam), cancellationToken);

        // Assert
        result.Value.ShouldBeEmpty();
    }
}
