using System.Text.Json.Nodes;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.Endpoints;
using Conferenti.Application.OpenApi.Examples;
using Conferenti.Application.Sessions.GetSessions;
using Conferenti.Domain.Sessions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Conferenti.Api.Endpoints.Sessions;

public class GetSessions : IEndpoint
{
    public void AddEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("sessions", GetSessionsAsync)
            .WithTags(ApiConstants.SessionApiTag)
            .WithSummary("Fetch sessions")
            .MapToApiVersion(ApiVersions.V1)
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                operation.Summary = "Fetch sessions";
                operation.Description = "Fetch sessions with pagination and filtering options.";

                operation.Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Successful operation",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Example = JsonNode.Parse(SessionOpenApiResponseExamples.SessionOkResponse)
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
                    ["401"] = new OpenApiResponse { Description = "Unauthorized" },
                    ["403"] = new OpenApiResponse { Description = "Forbidden" },
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
    private static async Task<Results<Ok<List<Session>>,
        UnauthorizedHttpResult,
        ForbidHttpResult,
        BadRequest<ProblemDetails>,
        InternalServerError<ProblemDetails>>> GetSessionsAsync(
        [AsParameters] SessionParam param,
        IQueryHandler<GetSessionsQuery, List<Session>> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var query = new GetSessionsQuery(param);
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
