using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace Conferenti.Domain.Tests.Unit.Speakers;

public class SpeakerTests
{
    [Fact]
    public void Create_ShouldSetPropertyValues()
    {
        // Act
        var speaker = SpeakerData.Speaker;

        // Assert
        speaker.Id.ShouldBe(SpeakerData.Id);
        speaker.Name.ShouldBe(SpeakerData.Name);
        speaker.Bio.ShouldBe(SpeakerData.Bio);
        speaker.Company.ShouldBe(SpeakerData.Company);
        speaker.Position.ShouldBe(SpeakerData.Position);
        speaker.PhotoUrl.ShouldBe(SpeakerData.PhotoUrl);
        speaker.SpeakerSessions.ShouldBeEmpty();
    }
}
