using System.Configuration;
using System.Reflection;
using System.Security.Claims;
using Asp.Versioning.ApiExplorer;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureKeyVaultEmulator.Aspire.Client;
using Conferenti.Api;
using Conferenti.Api.Endpoints;
using Conferenti.Api.ExceptionHandlers;
using Conferenti.Api.Helper;
using Conferenti.Api.Settings;
using Conferenti.Application;
using Conferenti.Infrastructure;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.JsonWebTokens;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;


const string AllowSpecificOrigins = "AllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

var telemetrySettings = builder.Configuration.GetSection("TelemetrySettings").Get<TelemetrySettings>();
var keyVaultSettings = builder.Configuration.GetSection("KeyVaultSettings").Get<KeyVaultSettings>();
var allowedOrigins = builder.Configuration.GetValue<string>("CorsSettings:AllowedOrigins")?.Split(";");

builder.Services.AddVaultService(builder.Configuration, keyVaultSettings!);

#pragma warning disable ASP0000
await using (var tempProvider = builder.Services.BuildServiceProvider())
{
    var secretClient = tempProvider.GetRequiredService<SecretClient>();
    builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.EnableAdaptiveSampling = false;
            options.EnableDebugLogger = true;
            // Do not set ConnectionString or InstrumentationKey for local development
        });

    await builder.ConfigureOpenTelemetry(secretClient!, telemetrySettings!);
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console(new RenderedCompactJsonFormatter())
        .WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces));
}
#pragma warning restore ASP0000

builder.Services
    .AddExceptionHandler<CustomExceptionHandler>()
    .AddProblemDetails(options =>
    {
        options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance =
                $"{context.HttpContext.Request.Method} {context.HttpContext.Request.GetEncodedPathAndQuery()}";
            if (context.HttpContext.Response.StatusCode == StatusCodes.Status500InternalServerError)
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            }
        };
    })
    .AddCors(
        options => options.AddPolicy(
            AllowSpecificOrigins,
            builder => builder
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(allowedOrigins!)))
    .AddEndpointsApiExplorer()
    .AddSwagger();

builder.Services.AddApplication();

await builder.Services.AddInfrastructure(builder.Configuration);
builder.Services
    .AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddAuthorization(options =>
{
    var readAccessPolicy = new AuthorizationPolicyBuilder()
        .RequireAssertion(context =>
        {
            return context.User.HasClaim(claim =>
                claim.Type == "scope" && claim.Value.Contains(Constants.ReadShowsScope));
        }).Build();

    var hasAdminAccessPolicy = new AuthorizationPolicyBuilder()
        .RequireAssertion(context =>
        {
            return context.User.HasClaim(claim =>
                claim.Type == "scope" && (IsCreateOrganizationScope(context.User) || IsReadShowScope(context.User)));
        }).Build();



    options.AddPolicy(Constants.AdminAccess, hasAdminAccessPolicy);
    options.AddPolicy(Constants.ReadAccess, readAccessPolicy);
})
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://kkho.eu.auth0.com/";
        options.Audience = "https://conferenti.com/api/";
#pragma warning disable CA5404
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidateIssuer = false;
#pragma warning restore CA5404
    });

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

var app = builder.Build()
    .MapEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseResponseCompression();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(AllowSpecificOrigins);

app.UseSwaggerSpecific(app.Environment, app.Services.GetRequiredService<IApiVersionDescriptionProvider>());
app.UseExceptionHandler();
app.UseStatusCodePages();


await app.RunAsync();
return;


#pragma warning disable IDE0061

static bool IsCreateOrganizationScope(ClaimsPrincipal user) => user.HasClaim(claim => claim.Type == "scope" && claim.Value.Contains(Constants.CreateOrganizationScope));

static bool IsReadShowScope(ClaimsPrincipal user) => user.HasClaim(claim => claim.Type == "scope" && claim.Value.Contains(Constants.ReadShowsScope));
#pragma warning restore IDE0061
