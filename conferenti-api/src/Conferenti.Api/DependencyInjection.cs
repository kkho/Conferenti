using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Security.KeyVault.Secrets;
using Conferenti.Api.Endpoints;
using Conferenti.Api.OpenApi;
using Conferenti.Api.Settings;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Azure.Identity;
using AzureKeyVaultEmulator.Aspire.Client;
using Conferenti.Api.Helper;

namespace Conferenti.Api;

public static class DependencyInjection
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1.0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("api-version"),
                    new MediaTypeApiVersionReader("api-version"));
            })
            .AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(setupAction =>
        {
            var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
            setupAction.IncludeXmlComments(xmlCommentsFullPath);
            setupAction.ExampleFilters();
        });

        services.AddSwaggerExamplesFromAssemblyOf<Program>();
    }

    public static void UseSwaggerSpecific(this IApplicationBuilder app, IHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        var urlPrefix = env.IsDevelopment() ? string.Empty : "/";
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "swagger/{documentName}/swagger.json"; //  can begin with slash
            // //not syntax error or poor attempt of stringInterpolation. {documentName} is required when using routeTemplate.
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers = [new() { Url = $"https://{httpReq.Host.Value}{urlPrefix}" }]; // slash between host and prefix
                });
            }
        });

        app.UseSwaggerUI(
            options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"../swagger/{description.GroupName}/swagger.json",  //does not begin with / for empty,
                        description.GroupName.ToUpperInvariant());
                }
            });
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var versionedGroup = app
            .MapGroup("v{apiVersion:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        app.AddEndpoints(versionedGroup);

        return app;
    }

    public static async Task ConfigureOpenTelemetry(this WebApplicationBuilder builder, SecretClient? secretClient, TelemetrySettings settings)
    {
        var useLocal = settings.UseLocal;

        if (!useLocal && secretClient != null)
        {
            var appInsightsConnectionString = await secretClient.GetSecretAsync(Constants.AppInsightsConnectionString);
            builder.Services.AddSingleton<TelemetryConfiguration>(sp =>
            {
                var config = TelemetryConfiguration.CreateDefault();
                config.ConnectionString = appInsightsConnectionString is not null
                    ? appInsightsConnectionString.Value.Value
                    : builder.Configuration["ApplicationInsights:ConnectionString"];
                return config;
            });
        }

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;

            if (useLocal)
            {
                logging.AddConsoleExporter();
            }
        });


        var openTelemetryBuilder = builder.Services
            .AddOpenTelemetry()
            .WithTracing(tracingBuilder =>
            {
                tracingBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (useLocal)
                {
                    tracingBuilder.AddConsoleExporter(); // Simulate traces
                }
            })
            .WithMetrics(metricsBuilder =>
            {
                metricsBuilder.AddAspNetCoreInstrumentation();

                if (useLocal)
                {
                    metricsBuilder.AddConsoleExporter(); // Simulate metrics
                }
            });

        if (!useLocal)
        {
            openTelemetryBuilder.UseAzureMonitor();
        }
    }

    public static void AddVaultService(this IServiceCollection service, IConfiguration configuration, KeyVaultSettings vaultSettings)
    {
        if (vaultSettings.BypassKeyVault)
        {
            return;
        }

        if (vaultSettings.UseEmulator)
        {
            var vaultUri = configuration.GetConnectionString("keyvault") ?? string.Empty;
            service.AddAzureKeyVaultEmulator(vaultUri, secrets: true, keys: true, certificates: false);
        }
        else
        {
            service.AddTransient(s => new SecretClient(new Uri(vaultSettings.VaultEndPoint), new DefaultAzureCredential()));
        }
    }
}
