using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Checkout.PublicApi;
using Modules.Checkout.PublicApi.Contracts;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;

namespace Modules.Checkout.Features.Features.StartCheckout;

public sealed class StartCheckoutRequestValidator : AbstractValidator<StartCheckoutRequest>
{
	public StartCheckoutRequestValidator()
	{
		RuleFor(x => x.Carrier).NotEmpty();
	}
}

public sealed class StartCheckoutEndpoint : IApiEndpoint
{
	public Asp.Versioning.ApiVersion Version => new(1.0);

	public void MapEndpoint(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
	{
		app.MapPost("/checkout", Handle)
			.WithTags("Checkout")
			.WithSummary("Consolidates the basket into a paid shipment.");
	}

	private static async Task<IResult> Handle(
		ClaimsPrincipal user,
		[FromBody] StartCheckoutRequest request,
		IValidator<StartCheckoutRequest> validator,
		ICheckoutModuleApi checkoutApi,
		CancellationToken cancellationToken)
	{
		var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null)
		{
			return Results.Unauthorized();
		}

		var validation = await validator.ValidateAsync(request, cancellationToken);
		if (!validation.IsValid)
		{
			return Results.ValidationProblem(validation.ToDictionary());
		}

		var result = await checkoutApi.StartAsync(userId, request, cancellationToken);
		return result.IsError ? result.Errors.ToProblem() : Results.Ok(result.Value);
	}
}
