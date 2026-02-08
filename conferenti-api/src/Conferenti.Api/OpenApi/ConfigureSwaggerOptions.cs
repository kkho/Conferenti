using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Conferenti.Api.OpenApi;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = $"Conferenti API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = "An API to manage conferences and speakers.",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "",
                        Email = string.Empty,
                    },
                });
        }

        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);

        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\". Prefix token with 'Bearer'",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            }] = new List<string>()
        });

        options.SchemaFilter<RequiredNotNullableSchemaFilter>();
        options.OperationFilter<MakeApiVersionHeaderMandatory>();
    }
}

internal sealed class MakeApiVersionHeaderMandatory : IOperationFilter
{
    public void Apply(Microsoft.OpenApi.Models.OpenApiOperation operation, OperationFilterContext context)
    {
        var versionParameter = operation.Parameters?.SingleOrDefault(p => p.Name == "api-version");

        if (versionParameter == null)
        {
            return;
        }

        versionParameter.Required = true;
    }
}

internal sealed class RequiredNotNullableSchemaFilter : ISchemaFilter
{
    public void Apply(Microsoft.OpenApi.Models.OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
        {
            return;
        }

        var requiredButNullableProperties = schema
            .Properties
            .Where(x => x.Value.Nullable && schema.Required.Contains(x.Key))
            .ToList();

        foreach (var property in requiredButNullableProperties)
        {
            property.Value.Nullable = false;
        }
    }
}
