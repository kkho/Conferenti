using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.Helpers;
using Conferenti.Domain.Abstractions;
using Conferenti.Domain.Sessions;
using Conferenti.Domain.Speakers;
using Microsoft.Extensions.Logging;

namespace Conferenti.Application.Sessions.PostSessions;
public class PostSessionCommandHandler(ISessionRepository sessionRepository, ILogger<PostSessionCommandHandler> logger) :
    ICommandHandler<PostSessionCommand, List<Session>>
{
    public async Task<Result<List<Session>>> Handle(PostSessionCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sessionRepository.UpsertSessionsAsync(command.Sessions, cancellationToken);
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
                JsonSerializer.Serialize(command, JsonHelper.IndentedJsonSerializerOptions),
                ex.GetType().FullName,
                ex.Message,
                ex.StackTrace);

            return Result.Failure<List<Session>>(DomainErrors.CreateError<Session>(ex.Message));
        }
    }
}
