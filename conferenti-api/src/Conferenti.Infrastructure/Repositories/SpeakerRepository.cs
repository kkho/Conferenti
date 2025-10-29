using Conferenti.Domain.Speakers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Conferenti.Infrastructure.Repositories;

public class SpeakerRepository : ISpeakerRepository
{
    private readonly Container _speakersContainer;

    public SpeakerRepository([FromKeyedServices("speakers")] Container speakerContainer)
    {
        _speakersContainer = speakerContainer;
    }

    public async Task<List<Speaker>> GetSpeakers(CancellationToken token)
    {
        var speakers = new List<Speaker>();
        var queryDef = new QueryDefinition("SELECT * FROM c");
        using var iterator = _speakersContainer.GetItemQueryIterator<Speaker>(queryDef);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(token).ConfigureAwait(false);
            speakers.AddRange(response.Resource);
        }

        return speakers;
    }

    public async Task<List<Speaker>> UpsertSpeakersAsync(List<Speaker> speakers, CancellationToken token)
    {
        var tasks = speakers.Select(speaker => _speakersContainer.UpsertItemAsync(speaker, new PartitionKey(speaker.Id), cancellationToken: token));
        var responses = await Task.WhenAll(tasks).ConfigureAwait(false);
        return responses.Select(r => r.Resource).ToList();
    }
}
