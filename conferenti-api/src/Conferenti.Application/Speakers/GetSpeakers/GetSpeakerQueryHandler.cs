using System.Text.Json;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.Helpers;
using Conferenti.Domain.Abstractions;
using Conferenti.Domain.Speakers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Conferenti.Application.Speakers.GetSpeakers;

internal sealed class GetSpeakerQueryHandler(
    ISpeakerRepository speakerRepository,
    ILogger<GetSpeakerQueryHandler> logger)
    : IQueryHandler<GetSpeakersQuery, List<Speaker>>
{
    public async Task<Result<List<Speaker>>> Handle(GetSpeakersQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var result = await speakerRepository.GetSpeakers(cancellationToken);
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

            return Result.Failure<List<Speaker>>(DomainErrors.CreateError<Speaker>(ex.Message));
        }
    }
}
