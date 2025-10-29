using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conferenti.Domain.Sessions;
using Conferenti.Domain.Speakers;

namespace Conferenti.Domain.Tests.Unit.Speakers;

internal static class SessionData
{
    public static readonly string Id = Guid.NewGuid().ToString();
    public const string Title = "Java"; 
    public const string Description = "A seasoned speaker on technology.";


    public static readonly Session Session = new Session
    {
        Id = Id,
        Title = Title,
        Description = Description
    };
}
