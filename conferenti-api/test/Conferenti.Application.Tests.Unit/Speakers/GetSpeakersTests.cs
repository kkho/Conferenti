using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conferenti.Application.Speakers.GetSpeakers;
using Conferenti.Domain.Speakers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace Conferenti.Application.Tests.Unit.Speakers;

public class GetSpeakersTests
{
    private readonly ISpeakerRepository _speakerRepositoryMock;
    private readonly ILogger<GetSpeakerQueryHandler> _loggerMock;
    private readonly GetSpeakerQueryHandler _handler;


    public GetSpeakersTests()
    {
        _speakerRepositoryMock = Substitute.For<ISpeakerRepository>();
        _loggerMock = Substitute.For<ILogger<GetSpeakerQueryHandler>>();
        _handler = new GetSpeakerQueryHandler(_speakerRepositoryMock, _loggerMock);
    }

    [Fact]
    public async Task? GeSpeakers_ShouldReturn_Success()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        _speakerRepositoryMock.GetSpeakers(cancellationToken).Returns([]);
        
        // Act
        var result = await _handler.Handle(new GetSpeakersQuery(), cancellationToken);

        // Assert
        result.Value.ShouldBeEmpty();
    }
}
