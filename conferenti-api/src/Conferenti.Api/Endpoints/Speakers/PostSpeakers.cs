using Conferenti.Api.Helper;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Application.Endpoints;
using Conferenti.Application.OpenApi.Examples;
using Conferenti.Application.Speakers.PostSpeakers;
using Conferenti.Domain.Speakers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Conferenti.Api.Endpoints.Speakers;

public class PostSpeakers : IEndpoint
{
    public void AddEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("speakers", UpsertSpeakersAsync)
            .WithTags(ApiConstants.SpeakerApiTag)
            .WithSummary("Post speakers data.")
            .Produces(200)
            .Produces(204) // No content
            .Produces(400) // Bad request
            .Produces(401) // Unauthorized
            .Produces(403) // Forbidden
            .Produces(404) // Not found
            .Produces(500) // Internal server error
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(Constants.AdminAccess)
            .WithOpenApi(op =>
            {
                op.RequestBody = new OpenApiRequestBody
                {
                    Required = true,
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Example = OpenApiAnyFactory.CreateFromJson(
                                SpeakerOpenApiResponseExamples
                                .UpsertSpeakersRequestBody)
                        }
                    }
                };
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

    private static async Task<Results<
        Ok<List<Speaker>>,
        UnauthorizedHttpResult,
        ForbidHttpResult,
        BadRequest<ProblemDetails>,
        NotFound<ProblemDetails>,
        InternalServerError<ProblemDetails>>> UpsertSpeakersAsync(
        [FromBody] Speaker[] speakers,
        ICommandHandler<PostSpeakerCommand, List<Speaker>> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var command = new PostSpeakerCommand(speakers.ToList());
        var result = await handler.Handle(command, cancellationToken);

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
