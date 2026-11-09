using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Modules.Catalog.Features.Caching;

/// <summary>
/// Allows output caching for endpoints that enforce authorization themselves.
/// </summary>
internal sealed class AuthenticatedOutputCachePolicy : IOutputCachePolicy
{
	public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
	{
		var request = context.HttpContext.Request;
		var isCacheableMethod = HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method);
		var isAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated == true;

		context.EnableOutputCaching = isCacheableMethod && isAuthenticated;
		context.AllowCacheLookup = context.EnableOutputCaching;
		context.AllowCacheStorage = context.EnableOutputCaching;
		context.AllowLocking = true;
		context.CacheVaryByRules.QueryKeys = "*";

		return ValueTask.CompletedTask;
	}

	public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
		=> ValueTask.CompletedTask;

	public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
	{
		var response = context.HttpContext.Response;
		if (response.StatusCode != StatusCodes.Status200OK ||
		    !StringValues.IsNullOrEmpty(response.Headers.SetCookie))
		{
			context.AllowCacheStorage = false;
		}

		return ValueTask.CompletedTask;
	}
}
