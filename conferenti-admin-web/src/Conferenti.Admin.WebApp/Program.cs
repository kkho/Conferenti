using Auth0.AspNetCore.Authentication;
using Conferenti.Admin.WebApp.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Security.Claims;
using Conferenti.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = builder.Configuration["Auth0:Domain"] ??
                         throw new InvalidOperationException("Auth0 Domain is not configured.");
        options.ClientId = builder.Configuration["Auth0:ClientId"] ??
                           throw new InvalidOperationException("Auth0 ClientId is not configured.");
        options.ClientSecret = builder.Configuration["Auth0:ClientSecret"] ??
                               throw new InvalidOperationException("Auth0 ClientSecret is not configured.");

        options.Scope = "openid profile email admin:execute";

        options.OpenIdConnectEvents = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is ClaimsIdentity identity)
                {
                    var accessToken = context.TokenEndpointResponse?.AccessToken;

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(accessToken);

                        var scopeClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "scope");
                        var roleClaims = jwtToken.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

                        if (scopeClaim != null)
                        {
                            identity.AddClaim(new Claim("scope", scopeClaim.Value));
                        }

                        foreach (var roleClaim in roleClaims)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                        }
                    }
                }
                return Task.CompletedTask;
            }
        };
    })
    .WithAccessToken(options =>
    {
        options.Audience = builder.Configuration["Auth0:Audience"] ?? "https://admin.conferenti.com/api/";
    });

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();
builder.Services.AddServiceBus(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri(returnUrl)
        .WithScope("admin:execute")
        .Build();

    await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/Account/AccessDenied", (HttpContext httpContext, string returnUrl = "/") =>
{
    httpContext.Response.Redirect("/unauthorized");
    return Task.CompletedTask;
});

app.MapGet("/Account/Logout", async (HttpContext httpContext) =>
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
        .WithRedirectUri("/")
        .Build();

    await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
