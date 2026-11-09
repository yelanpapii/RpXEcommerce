using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi;
using Modules.Common.API.ErrorHandling;
using Serilog;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

[SuppressMessage("Design", "MA0051:Method is too long")]
public static class DependencyInjection
{
    public static IServiceCollection AddCoreWebApiInfrastructure(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1.0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "RPX Ecommerce API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "JWT Authorization header. Enter: {token} (without Bearer)",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(_ =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer")] = []
                });
        });

        services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails();

        services.Configure<JsonOptions>(opt =>
        {
            opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }

    public static void AddCoreHostLogging(this WebApplicationBuilder builder)
    {
		builder.Host.UseSerilog((context, loggerConfiguration) =>
		{
			loggerConfiguration
				.ReadFrom.Configuration(context.Configuration)
				.Enrich.FromLogContext()
				.WriteTo.OpenTelemetry(options =>
				{
					options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
					options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc;
					options.ResourceAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
					{
						["service.name"] = builder.Environment.ApplicationName
					};
				});
		});
	}
}
