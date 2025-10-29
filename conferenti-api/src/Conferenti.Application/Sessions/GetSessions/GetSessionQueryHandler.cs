using System.Text.Json;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.Helpers;
using Conferenti.Domain.Abstractions;
using Conferenti.Domain.Sessions;
using Conferenti.Domain.Speakers;
using Microsoft.Extensions.Logging;

namespace Conferenti.Application.Sessions.GetSessions;
public class GetSessionQueryHandler(
    ISessionRepository sessionRepository,
    ILogger<GetSessionQueryHandler> logger)
    : IQueryHandler<GetSessionsQuery, List<Session>>
{
    public async Task<Result<List<Session>>> Handle(GetSessionsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sessionRepository.GetSessions(query.param, cancellationToken);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                " Handler: {HandlerName}" +
                "\nRequest: {Request}" +
                "\nExceptionType: {ExceptionType}" +
                "\nExceptionMessage: {ExceptionMessage}" +
                "\nStackTrace: {StackTrace}",
                GetType().Name,
                JsonSerializer.Serialize(query, JsonHelper.IndentedJsonSerializerOptions),
                ex.GetType().FullName,
                ex.Message,
                ex.StackTrace);

            return Result.Failure<List<Session>>(DomainErrors.CreateError<Session>(ex.Message));
        }
    }
}
