#nullable enable
using Microsoft.AspNetCore.Mvc;

namespace Conferenti.Domain.Sessions;

public class SessionParam
{
    [FromQuery]
    public string? Title { get; set; }

    [FromQuery]
    public string[]? Tags { get; set; }

    [FromQuery]
    public DateTime? StartTime { get; set; }

    [FromQuery]
    public DateTime? EndTime { get; set; }

    [FromQuery]
    public string? Room { get; set; }

    [FromQuery]
    public SessionLevel[]? Level { get; set; }

    [FromQuery]
    public SessionFormat? Format { get; set; }

    [FromQuery]
    public string? Language { get; set; }
}
