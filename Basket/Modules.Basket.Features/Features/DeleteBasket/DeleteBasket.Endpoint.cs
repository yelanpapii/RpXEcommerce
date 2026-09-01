using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Basket.Domain.Policies;
using Modules.Basket.Features.Features.Shared.Routes;
using Modules.Basket.PublicApi;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;

namespace Modules.Basket.Features.Features.DeleteBasket;

public sealed class DeleteBasketEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapDelete(RouteConsts.Basket, Handle)
			.RequireAuthorization(BasketPolicyConsts.DeletePolicy)
			.WithTags("Basket")
			.WithSummary("Deletes the current shopping basket.");
	}

	private static async Task<IResult> Handle(
		ClaimsPrincipal user,
		[FromServices] IBasketModuleApi handler,
		CancellationToken cancellationToken)
	{
		var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null)
		{
			return Results.Unauthorized();
		}

		var response = await handler.DeleteAsync(userId, cancellationToken);
		return response.IsError ? response.Errors.ToProblem() : Results.NoContent();
	}
}
