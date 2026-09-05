using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Asp.Versioning;
using Modules.Common.API.Abstractions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering and mapping endpoints in an ASP.NET Core application.
/// </summary>
public static class MapEndpointExtensions
{
    /// <summary>
    /// Registers all API endpoints implementing <see cref="IApiEndpoint"/> from the assembly containing the specified type <typeparamref name="T"/> into the service collection.
    /// </summary>
    /// <param name="services">The service collection to register the endpoints into.</param>
    /// <param name="marker">A type whose assembly will be scanned for <see cref="IApiEndpoint"/> implementations.</param>
    /// <returns>The modified service collection with registered endpoints.</returns>
    public static IServiceCollection RegisterApiEndpointsFromAssemblyContaining(this IServiceCollection services, Type marker)
    {
        var assembly = marker.Assembly;
        
        var endpointTypes = assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IApiEndpoint)) && t is { IsClass: true, IsAbstract: false, IsInterface: false });
        
        var serviceDescriptors = endpointTypes
            .Select(type => ServiceDescriptor.Transient(typeof(IApiEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);
        return services;
    }

    /// <summary>
    /// Maps all registered API endpoints implementing the <see cref="IApiEndpoint"/> interface to the specified web application.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance to which the endpoints will be mapped.</param>
    /// <returns>The same <see cref="WebApplication"/> instance to allow for method chaining.</returns>
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IApiEndpoint>>();

		var endpointsByVersion = endpoints.GroupBy(endpoint => endpoint.Version);

		foreach (var versionGroup in endpointsByVersion)
		{
			var version = versionGroup.Key;

			var versionSet = app.NewApiVersionSet()
				.HasApiVersion(version)
				.ReportApiVersions()
				.Build();

			var api = app.MapGroup("/api/v{version:apiVersion}")
				.WithApiVersionSet(versionSet)
				.MapToApiVersion(version);

			foreach (var endpoint in versionGroup)
			{
				endpoint.MapEndpoint(api);
			}
		}

		return app;
    }
}
