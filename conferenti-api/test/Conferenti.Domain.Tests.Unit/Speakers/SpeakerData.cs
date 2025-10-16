using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conferenti.Domain.Speakers;

namespace Conferenti.Domain.Tests.Unit.Speakers;

internal static class SpeakerData
{
    public static readonly string Id = Guid.NewGuid().ToString();
    public const string Name = "John Doe"; 
    public const string Bio = "A seasoned speaker on technology.";
    public const string Company = "Tech Corp";
    public const string Position = "Senior Developer";
    public const string PhotoUrl = "https://example.com/photo.jpg";


    public static readonly Speaker Speaker = new Speaker
    {
        Id = Id,
        Name = Name,
        Bio = Bio,
        Company = Company,
        Position = Position,
        PhotoUrl = PhotoUrl,
        SpeakerSessions = []
    };
}
