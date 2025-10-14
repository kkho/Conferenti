namespace Conferenti.Domain.Sessions;

/// <summary>
/// Detailed information about a session given by a speaker
/// </summary>
public class Session
{
    /// <summary>
    /// The Session id used to get session details
    /// </summary>
    public string SessionId { get; set; }

    /// <summary>
    /// Title of the session
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Slug of the session
    /// </summary>
    public string Slug { get; set; }

    /// <summary>
    /// Tags associated with the session
    /// </summary>
    public string[] Tags { get; set; }

    /// <summary>
    /// Description of the session
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Start time of the session
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// End time of the session
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Room where the session is held
    /// </summary>
    public string Room { get; set; }

    /// <summary>
    /// Level of the session (e.g., Beginner, Intermediate, Advanced)
    /// </summary>
    public SessionLevel Level { get; set; }

    /// <summary>
    /// Format of the session (e.g., Workshop, Lecture, Panel)
    /// </summary>
    public SessionFormat Format { get; set; }

    /// <summary>
    /// Language in which the session is conducted
    /// </summary>
    public string Language { get; set; }
}