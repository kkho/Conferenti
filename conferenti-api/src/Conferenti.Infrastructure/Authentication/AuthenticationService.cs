using System.Net.Http.Json;
using System.Security.Claims;
using Conferenti.Application.Abstractions.Authentication;
using Conferenti.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Conferenti.Infrastructure.Authentication;
public class AuthenticationService : IAuthenticationService
{
    private readonly Auth0Settings _auth0Settings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthenticationService(
        IOptions<Auth0Settings> auth0Settings,
        IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory
    )
    {
        _auth0Settings = auth0Settings.Value;
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetServiceTokenAsync(CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient("Auth0");
        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _auth0Settings.AiClientId,
            ["client_secret"] = _auth0Settings.AiClientSecret,
            ["audience"] = _auth0Settings.Audience
        };

        using var formContent = new FormUrlEncodedContent(tokenRequest);
        var response = await httpClient.PostAsync(
            $"{_auth0Settings.Authority}oauth/token",
            formContent,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
        return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Failed to acquire service token");
    }

    public AuthenticationResult? GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;
        var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var claims = user.Claims.ToDictionary(c => c.Type, c => c.Value);
        return new AuthenticationResult(userId, email ?? string.Empty, claims);
    }

    public string? GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
    }
}
