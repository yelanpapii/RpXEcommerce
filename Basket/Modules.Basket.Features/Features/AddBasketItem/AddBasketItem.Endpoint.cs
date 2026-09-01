using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Basket.Domain.Policies;
using Modules.Basket.Features.Features.Shared.Routes;
using Modules.Basket.PublicApi;
using Modules.Basket.PublicApi.Contracts;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;

namespace Modules.Basket.Features.Features.AddBasketItem;

public sealed class AddBasketItemEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost(RouteConsts.Basket, Handle)
			.RequireAuthorization(BasketPolicyConsts.CreatePolicy)
			.WithTags("Basket")
			.WithSummary("Adds an item to the shopping basket.");
	}

	private static async Task<IResult> Handle(
		ClaimsPrincipal user,
		[FromBody] AddBasketItemRequest request,
		[FromServices] IBasketModuleApi handler,
		CancellationToken cancellationToken)
	{
		var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null)
		{
			return Results.Unauthorized();
		}

		var response = await handler.AddItemAsync(userId, request, cancellationToken);
		return response.IsError ? response.Errors.ToProblem() : Results.Ok(response.Value);
	}
}
