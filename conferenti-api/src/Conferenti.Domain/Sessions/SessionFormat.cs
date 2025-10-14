namespace Conferenti.Domain.Sessions;

/// <summary>
/// Represents the format of a session in an event or conference.
/// </summary>
/// <remarks>This enumeration defines the possible types of sessions, such as lectures, workshops, panels, and
/// keynotes. It can be used to categorize or filter sessions based on their format.</remarks>
public enum SessionFormat
{
    Lecture,
    Workshop,
    Panel,
    Keynote,
    Presentation
}