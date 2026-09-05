using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Shipments.Domain.Policies;
using Modules.Shipments.Features.Features.Shared.Routes;

namespace Modules.Shipments.Features.Features.DispatchShipment;

public class DispatchShipmentEndpoint : IApiEndpoint
{
	public Asp.Versioning.ApiVersion Version => new(1.0);
	public void MapEndpoint(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
	{
		app.MapPost(RouteConsts.DispatchShipment, Handle)
			.RequireAuthorization(ShipmentPolicyConsts.UpdatePolicy)
			.WithTags("Shipments")
			.WithDescription("Dispatches a shipment.");
	}

	private static async Task<IResult> Handle(
		[FromRoute] string shipmentNumber,
		IDispatchShipmentHandler handler,
		CancellationToken cancellationToken)
	{
		var response = await handler.HandleAsync(shipmentNumber, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.NoContent();
	}
}
