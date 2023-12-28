using System.Text;
using System.Text.Json;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BggExt;

public static class SwaggerConfiguration
{
    public static WebApplication UseConfiguredSwaggerUI(this WebApplication app)
    {
        app
            .UseSwagger()
            .UseSwaggerUI(opts =>
            {
                var descriptions = app.DescribeApiVersions();
                foreach (var desc in descriptions)
                {
                    var url = $"/swagger/{desc.GroupName}/swagger.json";
                    var name = desc.GroupName.ToUpperInvariant();
                    opts.SwaggerEndpoint(url, $"Test API {name}");
                }
            });

        return app;
    }

    public static IServiceCollection AddConfiguredSwaggerUI(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        Scheme = "bearer",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http
                    });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            })
            .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
            .AddApiVersioning(opts =>
            {
                opts.AssumeDefaultVersionWhenUnspecified = true;
                opts.DefaultApiVersion = new ApiVersion(1, 0);
                opts.ReportApiVersions = true;
                opts.ApiVersionReader = opts.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(opts =>
            {
                opts.GroupNameFormat = "'v'V";
                opts.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConfigureSwaggerOptions" /> class.
    /// </summary>
    /// <param name="provider">
    ///     The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger
    ///     documents.
    /// </param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        this.provider = provider;
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in provider.ApiVersionDescriptions)
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder("An example application with OpenAPI, Swashbuckle, and API versioning.");
        var info = new OpenApiInfo
        {
            Title = "BGGExt API",
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated) text.Append(" This API version has been deprecated.");

        if (description.SunsetPolicy is SunsetPolicy policy)
        {
            if (policy.Date is { } when)
                text.Append(" The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');

            if (policy.HasLinks)
            {
                text.AppendLine();

                for (var i = 0; i < policy.Links.Count; i++)
                {
                    var link = policy.Links[i];

                    if (link.Type == "text/html")
                    {
                        text.AppendLine();

                        if (link.Title.HasValue) text.Append(link.Title.Value).Append(": ");

                        text.Append(link.LinkTarget.OriginalString);
                    }
                }
            }
        }

        info.Description = text.ToString();

        return info;
    }
}

public class SwaggerDefaultValues : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1752#issue-663991077
        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
        {
            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/b7cf75e7905050305b115dd96640ddd6e74c7ac9/src/Swashbuckle.AspNetCore.SwaggerGen/SwaggerGenerator/SwaggerGenerator.cs#L383-L387
            var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
            var response = operation.Responses[responseKey];

            foreach (var contentType in response.Content.Keys)
                if (!responseType.ApiResponseFormats.Any(x => x.MediaType == contentType))
                    response.Content.Remove(contentType);
        }

        if (operation.Parameters == null) return;

        // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
        // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

            if (parameter.Description == null) parameter.Description = description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null &&
                description.DefaultValue != null &&
                description.DefaultValue is not DBNull &&
                description.ModelMetadata is ModelMetadata modelMetadata)
            {
                // REF: https://github.com/Microsoft/aspnet-api-versioning/issues/429#issuecomment-605402330
                var json = JsonSerializer.Serialize(description.DefaultValue, modelMetadata.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            parameter.Required |= description.IsRequired;
        }
    }
}
