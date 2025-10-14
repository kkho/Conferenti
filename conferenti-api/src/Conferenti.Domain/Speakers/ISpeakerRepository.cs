namespace Conferenti.Domain.Speakers;

public interface ISpeakerRepository
{
    Task<List<Speaker>> GetSpeakers(CancellationToken token);

    Task<List<Speaker>> UpsertSpeakersAsync(List<Speaker> speakers, CancellationToken token);
}
