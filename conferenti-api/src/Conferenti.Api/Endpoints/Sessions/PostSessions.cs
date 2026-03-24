using System.Text.Json.Nodes;
using Conferenti.Api.Helper;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.Endpoints;
using Conferenti.Application.OpenApi.Examples;
using Conferenti.Application.Sessions.PostSessions;
using Conferenti.Domain.Sessions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Conferenti.Api.Endpoints.Sessions;

public class PostSessions : IEndpoint
{
    public void AddEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("sessions", UpsertSessionsAsync)
            .WithTags(ApiConstants.SessionApiTag)
            .WithSummary("Post sessions data.")
            .Produces(200)
            .Produces(204) // No content
            .Produces(400) // Bad request
            .Produces(401) // Unauthorized
            .Produces(403) // Forbidden
            .Produces(404) // Not found
            .Produces(500) // Internal server error
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(Constants.AdminAccess)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Required = true,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {

                            Example = JsonNode.Parse(SessionOpenApiResponseExamples.UpsertSessionRequestBody)
                        }
                    }
                };
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

    private static async Task<Results<
        Ok<List<Session>>,
        UnauthorizedHttpResult,
        ForbidHttpResult,
        BadRequest<ProblemDetails>,
        NotFound<ProblemDetails>,
        InternalServerError<ProblemDetails>>> UpsertSessionsAsync(
        [FromBody] Session[] sessions,
        ICommandHandler<PostSessionCommand, List<Session>> handler,
        HttpContext httpContext,
        CancellationToken token)
    {
        var command = new PostSessionCommand(sessions.ToList());
        var result = await handler.Handle(command, cancellationToken: token);

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
}
