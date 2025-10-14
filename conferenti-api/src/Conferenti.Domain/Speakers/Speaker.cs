using Conferenti.Domain.Sessions;
using System.Text.Json.Serialization;

namespace Conferenti.Domain.Speakers;

/// <summary>
/// Response model for speaker
/// </summary>
public class Speaker
{
    /// <summary>
    /// The Speaker id used to get session details
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Full name of the speaker
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Position of the speaker in the company
    /// </summary>
    [JsonPropertyName("position")]
    public string Position { get; set; }

    /// <summary>
    /// Company associated with the speaker
    /// </summary>
    [JsonPropertyName("company")]
    public string Company { get; set; }

    /// <summary>
    /// The biography of the speaker
    /// </summary>
    [JsonPropertyName("bio")]
    public string Bio { get; set; }

    /// <summary>
    /// Url of speaker. Usually stored in CDN. 
    /// </summary>
    [JsonPropertyName("photoUrl")]
    public string PhotoUrl { get; set; }

    /// <summary>
    /// Speaker sessions associated with the event.
    /// </summary>
    [JsonPropertyName("sessions")]
    public Session[] Sessions { get; set; }
}
