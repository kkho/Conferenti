using System.Text.Json.Serialization;

namespace Conferenti.Domain.Sessions;

/// <summary>
/// Detailed information about a session given by a speaker
/// </summary>
public class Session
{
    /// <summary>
    /// The Session id used to get session details
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; }

    /// <summary>
    /// Title of the session
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Slug of the session
    /// </summary>
    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    /// <summary>
    /// Tags associated with the session
    /// </summary>
    [JsonPropertyName("tags")]
    public string[] Tags { get; set; }

    /// <summary>
    /// Description of the session
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    /// Start time of the session
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// End time of the session
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Room where the session is held
    /// </summary>
    [JsonPropertyName("room")]
    public string Room { get; set; }

    /// <summary>
    /// Level of the session (e.g., Beginner, Intermediate, Advanced)
    /// </summary>
    [JsonPropertyName("level")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SessionLevel Level { get; set; }

    /// <summary>
    /// Format of the session (e.g., Workshop, Lecture, Panel)
    /// </summary>
    [JsonPropertyName("format")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SessionFormat Format { get; set; }

    /// <summary>
    /// Language in which the session is conducted
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; set; }

    /// <summary>
    /// The IDs of the speakers presenting the session
    /// </summary>
    [JsonPropertyName("speakerIds")]
    public string[] SpeakerIds { get; set; }
}
