using System.Text.Json.Serialization;

namespace Conferenti.Domain.Abstractions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorType
{
    None = 0,
    Failure = 1,
    Validation = 2,
    NotFound = 3,
    Conflict = 4,
    Concurrency = 5,
    Problem = 6,
    NoContent = 7
}
