namespace Conferenti.Application.Abstractions.Authentication;
/// <summary>
/// Service for handling authentication operations
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Gets authentication token for AI service communication (M2M)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bearer token for service-to-service authentication</returns>
    Task<string> GetServiceTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current authenticated user from HTTP context
    /// Token validation is handled by JWT middleware
    /// </summary>
    /// <returns>User information if authenticated, null otherwise</returns>
    AuthenticationResult? GetCurrentUser();

    /// <summary>
    /// Gets the current authenticated user's identifier
    /// </summary>
    /// <returns>User ID from current HTTP context</returns>
    string? GetCurrentUserId();
}

/// <summary>
/// Result of authentication operation
/// </summary>
public record AuthenticationResult(
    string UserId,
    string Email,
    IReadOnlyDictionary<string, string> Claims
);
