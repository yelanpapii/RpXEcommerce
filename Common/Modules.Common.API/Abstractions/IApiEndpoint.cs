using Asp.Versioning;
using Microsoft.AspNetCore.Routing;


namespace Modules.Common.API.Abstractions;

/// <summary>
/// Represents an interface defining the contract for mapping API endpoints into a web application.
/// </summary>
public interface IApiEndpoint
{
	/// <summary>
	/// Gets the API version associated with this endpoint.
	/// </summary>
	ApiVersion Version { get; }

	/// <summary>
	/// Maps the API endpoint for the specified web application instance.
	/// </summary>
	/// <param name="app">The web application instance where the API endpoint will be mapped.</param>
	void MapEndpoint(IEndpointRouteBuilder app);
}
