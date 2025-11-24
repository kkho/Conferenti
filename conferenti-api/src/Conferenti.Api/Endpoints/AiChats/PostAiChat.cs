using System.Drawing.Text;
using Conferenti.Application.AiAgents;
using Conferenti.Application.Endpoints;
using Conferenti.Domain.AiAgents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Conferenti.Api.Endpoints.AiChats;

public class PostAiChat : IEndpoint
{
    public void AddEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/ai/chat", ChatAsync)
            .WithTags("AI")
            .WithSummary("Send message to AI agent")
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    private static async Task<Results<Ok<AiChatResponse>, BadRequest<ProblemDetails>>>
        ChatAsync(
            [FromBody] AiChatRequest request,
            IAiAgentService aiAgentService,
            CancellationToken cancellationToken)
    {
        var response = await aiAgentService.SendChatMessageAsync(request, cancellationToken);

        if (!response.Success)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "AI Agent Error",
                Detail = response.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return TypedResults.Ok(response);
    }
}
