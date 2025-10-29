using System.Text;
using Conferenti.Domain.Sessions;
using Conferenti.Domain.Speakers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Conferenti.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly Container _sessionsContainer;

    public SessionRepository([FromKeyedServices("sessions")] Container sessionContainer)
    {
        _sessionsContainer = sessionContainer;
    }

    public async Task<List<Session>> GetSessions(SessionParam param, CancellationToken token)
    {
        var sessions = new List<Session>();
        var queryBuilder = new StringBuilder("SELECT * FROM c WHERE 1=1");
        var queryDef = new QueryDefinition(queryBuilder.ToString());
        queryDef = BuildQueryParameters(param, queryBuilder, queryDef);

        using var iterator = _sessionsContainer.GetItemQueryIterator<Session>(queryDef);

        while(iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(token).ConfigureAwait(false);
            sessions.AddRange(response.Resource);
        }

        return sessions;
    }

    private static QueryDefinition BuildQueryParameters(SessionParam param, StringBuilder queryBuilder,
        QueryDefinition queryDef)
    {
        if (!string.IsNullOrEmpty(param.Title))
        {
            queryBuilder.Append(" AND CONTAINS(LOWER(c.title), LOWER(@title))");
            queryDef = new QueryDefinition(queryBuilder.ToString())
                .WithParameter("@title", param.Title);
        }

        if (param.Tags != null && param.Tags.Length > 0)
        {
            queryBuilder.Append(" AND ARRAY_CONTAINS(@tags, c.tags, true)");
            queryDef = new QueryDefinition(queryBuilder.ToString())
                .WithParameter("@tags", param.Tags);
        }

        if (param.StartTime != default)
        {
            queryBuilder.Append(" AND c.startTime >= @startTime");
            queryDef = new QueryDefinition(queryBuilder.ToString())
                .WithParameter("@startTime", param.StartTime);
        }

        if (param.EndTime != default)
        {
            queryBuilder.Append(" AND c.endTime <= @endTime");
            queryDef = new QueryDefinition(queryBuilder.ToString())
                .WithParameter("@endTime", param.EndTime);
        }

        if (!string.IsNullOrEmpty(param.Room))
        {
            queryBuilder.Append(" AND c.room = @room");
            queryDef = new QueryDefinition(queryBuilder.ToString())
                .WithParameter("@room", param.Room);
        }

        if (param.Level != null && param.Level.Length > 0)
        {
            queryBuilder.Append(" AND ARRAY_CONTAINS(@levels, c.level)");
            queryDef = new QueryDefinition(queryBuilder.ToString())
                .WithParameter("@levels", param.Level.Select(l => l.ToString()).ToArray());
        }

        if (param.Format != default)
        {
            queryBuilder.Append(" AND c.format = @format");
            queryDef = new QueryDefinition(queryBuilder.ToString())
                .WithParameter("@format", param.Format.ToString());
        }

        if (!string.IsNullOrEmpty(param.Language))
        {
            queryBuilder.Append(" AND c.language = @language");
            queryDef = new QueryDefinition(queryBuilder.ToString())
                .WithParameter("@language", param.Language);
        }

        return queryDef;
    }

    public async Task<List<Session>> UpsertSessionsAsync(List<Session> sessions, CancellationToken token)
    {
        var tasks = sessions.Select(session => _sessionsContainer.UpsertItemAsync(session, new PartitionKey(session.Id), cancellationToken: token));
        var responses = await Task.WhenAll(tasks).ConfigureAwait(false);
        return responses.Select(r => r.Resource).ToList();
    }
}
