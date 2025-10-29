namespace Conferenti.Domain.Sessions;
public interface ISessionRepository
{
    Task<List<Session>> GetSessions(SessionParam param, CancellationToken token);

    Task<List<Session>> UpsertSessionsAsync(List<Session> sessions, CancellationToken token);
}
