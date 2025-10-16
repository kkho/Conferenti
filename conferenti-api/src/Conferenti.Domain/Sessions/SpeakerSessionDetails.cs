using Conferenti.Domain.Speakers;

namespace Conferenti.Domain.Sessions;

public class SpeakerSessionDetails
{
    public Session Session { get; set; }

    public Speaker[] Speaker { get; set; }
}