using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Modules.Common.Application.Messaging;
using Modules.Common.Infrastructure.Configuration;
using Modules.Common.Infrastructure.Messaging;
using Modules.Common.Infrastructure.Policies;
using NATS.Net;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string[] activityModuleNames)
    {
        services.AddMemoryCache();

        services.AddOptions<NatsOptions>()
            .Configure(options =>
            {
                options.Url = configuration.GetConnectionString("Nats")
                    ?? configuration["Nats:Url"]
                    ?? "nats://localhost:4222";
                options.Stream = configuration["Nats:Stream"] ?? options.Stream;
                options.SubjectPrefix = configuration["Nats:SubjectPrefix"] ?? options.SubjectPrefix;
                options.StockInitializationSubject = configuration["Nats:StockInitializationSubject"] ?? options.StockInitializationSubject;
                options.StockInitializationDurableConsumer = configuration["Nats:StockInitializationDurableConsumer"] ?? options.StockInitializationDurableConsumer;
            });

		services.AddSingleton(sp =>
			new NatsClient(sp.GetRequiredService<IOptions<NatsOptions>>().Value.Url));

		services.AddSingleton<INatsPublisher, NatsJetStreamPublisher>();
		services.AddHostedService<NatsStreamProvisioner>();

		services.AddHostOpenTelemetry(activityModuleNames);

        services.AddJwtAuthentication(configuration);
        services.AddClaimsAuthorization();

        return services;
    }

    private static IServiceCollection AddHostOpenTelemetry(
        this IServiceCollection services,
        params string[] activityModuleNames)
    {
        services
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
	            metrics.AddAspNetCoreInstrumentation()
		            .AddHttpClientInstrumentation()
		            .AddRuntimeInstrumentation();
            })
            .ConfigureResource(resource => resource.AddService("ShippingModularMonolith"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddSource(activityModuleNames);

                tracing.AddOtlpExporter();
            });

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AuthConfiguration>()
            .Bind(configuration.GetSection(nameof(AuthConfiguration)));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["AuthConfiguration:Issuer"],
            ValidAudience = configuration["AuthConfiguration:Audience"],
#pragma warning disable S6781
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthConfiguration:Key"]  ?? throw new InvalidOperationException("JWT key is not configured")))
#pragma warning restore S6781
        };

        services.AddSingleton(tokenValidationParameters);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
	            options.TokenValidationParameters = tokenValidationParameters;
            });

        return services;
    }

    private static IServiceCollection AddClaimsAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<AuthorizationOptions>, AuthorizationConfigureOptions>();
        services.AddAuthorization();

        return services;
    }
}
