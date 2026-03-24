using Conferenti.Domain.AiAgents;

namespace Conferenti.Application.AiAgents;
public interface IAiAgentService
{
    /// <summary>
    /// Service for communicating with Ai Agent.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AiChatResponse> SendChatMessageAsync(AiChatRequest request, CancellationToken cancellationToken = default);
}
