using System.Text.Json.Nodes;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.Endpoints;
using Conferenti.Application.OpenApi.Examples;
using Conferenti.Application.Speakers.GetSpeakers;
using Conferenti.Domain.Speakers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
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
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                operation.Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Ok",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Example = JsonNode.Parse(SpeakerOpenApiResponseExamples.SpeakersOkResponse)
                            }
                        }
                    },
                    ["400"] = new OpenApiResponse
                    {
                        Description = "Bad Request",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Example = JsonNode.Parse(OpenApiSharedResponseExamples.BadRequest)
                            }
                        }
                    },
                    ["401"] = new OpenApiResponse
                    {
                        Description = "Unauthorized",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Example = JsonNode.Parse(OpenApiSharedResponseExamples.Unauthorized)
                            }
                        }
                    },
                    ["403"] = new OpenApiResponse
                    {
                        Description = "Forbidden",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Example = JsonNode.Parse(OpenApiSharedResponseExamples.Forbidden)
                            }
                        }
                    },
                    ["404"] = new OpenApiResponse
                    {
                        Description = "Not Found",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Example = JsonNode.Parse(OpenApiSharedResponseExamples.NotFound)
                            }
                        }
                    },
                    ["500"] = new OpenApiResponse
                    {
                        Description = "Internal Server Error",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Example = JsonNode.Parse(OpenApiSharedResponseExamples.InternalServerError)
                            }
                        }
                    }
                };

                return Task.CompletedTask;
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
