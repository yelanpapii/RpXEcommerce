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

namespace Modules.Basket.Features.Features.UpdateBasketItem;

public sealed class UpdateBasketItemEndpoint : IApiEndpoint
{
	public Asp.Versioning.ApiVersion Version => new(1.0);
	public void MapEndpoint(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
	{
		app.MapPut(RouteConsts.Basket, Handle)
			.RequireAuthorization(BasketPolicyConsts.UpdatePolicy)
			.WithTags("Basket")
			.WithSummary("Updates an item quantity in the shopping basket.");
	}

	private static async Task<IResult> Handle(
		ClaimsPrincipal user,
		[FromBody] UpdateBasketItemRequest request,
		[FromServices] IBasketModuleApi handler,
		CancellationToken cancellationToken)
	{
		var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null)
		{
			return Results.Unauthorized();
		}

		var response = await handler.UpdateItemAsync(userId, request, cancellationToken);
		return response.IsError ? response.Errors.ToProblem() : Results.Ok(response.Value);
	}
}
