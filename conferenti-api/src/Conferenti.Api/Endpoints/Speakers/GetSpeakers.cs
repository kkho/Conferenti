using Conferenti.Api.Helper;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.Exceptions.Examples;
using Conferenti.Application.Speakers.GetSpeakers;
using Conferenti.Domain.Speakers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Conferenti.Api.Endpoints.Speakers;

public class GetSpeakers : IEndpoint
{
    public void AddEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("speakers", GetSpeakersAsync)
            .WithTags(ApiConstants.SpeakerApiTag)
            .WithSummary("Fetch speakers")
            .MapToApiVersion(ApiVersions.V1)
            .WithOpenApi(op =>
            {
                op.Responses["200"] = new OpenApiResponse
                {
                    Description = "Ok",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Example = OpenApiAnyFactory.CreateFromJson(SpeakerOpenApiResponseExamples.SpeakersOkResponse)
                        }
                    }
                };
                op.Responses["400"] = new OpenApiResponse
                {
                    Description = "Bad Request",
                    Content =
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Example = OpenApiAnyFactory.CreateFromJson(OpenApiSharedResponseExamples.BadRequest)
                            }
                        }
                };
                op.Responses["401"] = new OpenApiResponse
                {
                    Description = "Unauthorized",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Example = OpenApiAnyFactory.CreateFromJson(OpenApiSharedResponseExamples.Unauthorized)
                        }
                    }
                };
                op.Responses["403"] = new OpenApiResponse
                {
                    Description = "Forbidden",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Example = OpenApiAnyFactory.CreateFromJson(OpenApiSharedResponseExamples.Forbidden)
                        }
                    }
                };
                op.Responses["404"] = new OpenApiResponse
                {
                    Description = "Not Found",
                    Content =
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Example = OpenApiAnyFactory.CreateFromJson(OpenApiSharedResponseExamples.NotFound)
                            }
                        }
                };
                op.Responses["500"] = new OpenApiResponse
                {
                    Description = "Internal Server Error",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Example = OpenApiAnyFactory.CreateFromJson(OpenApiSharedResponseExamples.InternalServerError)
                        }
                    }
                };
                return op;
            });
    }

#pragma warning disable S1172
    private static async Task<Results<
        Ok<List<Speaker>>,
        UnauthorizedHttpResult,
        ForbidHttpResult,
        BadRequest<ProblemDetails>,
        InternalServerError<ProblemDetails>>> GetSpeakersAsync(
        IQueryHandler<GetSpeakersQuery, List<Speaker>> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var query = new GetSpeakersQuery();
        var result = await handler.Handle(query, cancellationToken);

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        var problemDetail = result.Error.ToProblemDetails(httpContext);
        if (problemDetail.Status is StatusCodes.Status400BadRequest)
        {
            return TypedResults.BadRequest(problemDetail);
        }

        return TypedResults.InternalServerError(problemDetail);
    }
#pragma warning restore
}
