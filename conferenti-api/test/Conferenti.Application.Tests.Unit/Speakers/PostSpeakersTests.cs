using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conferenti.Application.Speakers.GetSpeakers;
using Conferenti.Application.Speakers.PostSpeakers;
using Conferenti.Domain.Speakers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace Conferenti.Application.Tests.Unit.Speakers;

public class PostSpeakersTests
{
    private readonly PostSpeakerCommandHandler _handler;
    private readonly ISpeakerRepository _speakerRepositoryMock;
    private readonly ILogger<PostSpeakerCommandHandler> _loggerMock;

    public PostSpeakersTests()
    {
        _speakerRepositoryMock = Substitute.For<ISpeakerRepository>();
        _loggerMock = Substitute.For<ILogger<PostSpeakerCommandHandler>>();
        _handler = new PostSpeakerCommandHandler(_speakerRepositoryMock, _loggerMock);
    }

    [Fact]
    public async Task PostSpeaker_With_Data_Should_Return_Valid_Response()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var name = "Jane Doe";
        var bio = "An expert in cloud computing.";
        var company = "Cloud Inc";
        var position = "Cloud Architect";
        var photoUrl = "http://someurl.com/jane";

        var command = new PostSpeakerCommand([
            new Speaker()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Bio = bio,
                Company = company,
                Position = position,
                PhotoUrl = photoUrl
            }
        ]);

        _speakerRepositoryMock.UpsertSpeakersAsync(command.Speakers, cancellationToken)
            .Returns(command.Speakers);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        var speaker = result.Value.First();
        speaker.Id.ShouldNotBeNullOrEmpty();
        speaker.Id.ShouldNotBe(Guid.Empty.ToString());
        speaker.Name.ShouldBe(name);
        speaker.Bio.ShouldBe(bio);
        speaker.Company.ShouldBe(company);
        speaker.Position.ShouldBe(position);
        speaker.PhotoUrl.ShouldBe(photoUrl);
    }
}
