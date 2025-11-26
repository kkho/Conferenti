using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Conferenti.Application.Abstractions.Authentication;
using Conferenti.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Conferenti.Infrastructure.Authentication;
public class AuthenticationService(
    Auth0Settings auth0Settings,
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory)
    : IAuthenticationService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };


    public async Task<string> GetServiceTokenAsync(CancellationToken cancellationToken = default)
    {
        var httpClient = httpClientFactory.CreateClient("Auth0");
        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = auth0Settings.AiClientId,
            ["client_secret"] = auth0Settings.AiClientSecret,
            ["audience"] = auth0Settings.Audience
        };

        using var formContent = new FormUrlEncodedContent(tokenRequest);
        var response = await httpClient.PostAsync(
            $"{auth0Settings.Authority}oauth/token",
            formContent,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken, options: _jsonSerializerOptions);
        return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Failed to acquire service token");
    }

    public AuthenticationResult? GetCurrentUser()
    {
        var user = httpContextAccessor.HttpContext?.User;

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
        var user = httpContextAccessor.HttpContext?.User;
        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
    }
}
