using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace Conferenti.Domain.Tests.Unit.Speakers;

public class SessionTests
{
    [Fact]
    public void Create_ShouldSetPropertyValues()
    {
        // Act
        var session = SessionData.Session;

        // Assert
        session.Id.ShouldBe(SessionData.Id);
        session.Title.ShouldBe(SessionData.Title);
        session.Description.ShouldBe(SessionData.Description);
        session.SpeakerIds.ShouldBeEmpty();
    }
}
