using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Domain.Sessions;

namespace Conferenti.Application.Sessions.GetSessions;

public record GetSessionsQuery(SessionParam param) : IQuery<List<Session>>;
