using System.Text.Json;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.Helpers;
using Conferenti.Domain.Abstractions;
using Conferenti.Domain.Speakers;
using Microsoft.Extensions.Logging;

namespace Conferenti.Application.Speakers.PostSpeakers;

public class PostSpeakerCommandHandler(ISpeakerRepository speakerRepository, ILogger<PostSpeakerCommandHandler> logger) : ICommandHandler<PostSpeakerCommand, List<Speaker>>
{

    public async Task<Result<List<Speaker>>> Handle(PostSpeakerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await speakerRepository.UpsertSpeakersAsync(command.Speakers, cancellationToken);
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

            return Result.Failure<List<Speaker>>(DomainErrors.CreateError<Speaker>(ex.Message));
        }
    }
}
