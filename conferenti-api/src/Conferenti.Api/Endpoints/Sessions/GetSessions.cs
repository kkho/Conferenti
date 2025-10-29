using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.OpenApi.Examples;
using Conferenti.Application.Sessions.GetSessions;
using Conferenti.Domain.Sessions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
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
            .WithOpenApi(op =>
            {
                op.Responses["200"] = new OpenApiResponse
                {
                    Description = "Ok",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Example = OpenApiAnyFactory.CreateFromJson(SessionOpenApiResponseExamples.SessionOkResponse)
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
                            Example = OpenApiAnyFactory.CreateFromJson(
                                OpenApiSharedResponseExamples.InternalServerError)
                        }
                    }
                };
                return op;
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
