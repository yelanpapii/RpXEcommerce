using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Shipments.Domain.Policies;
using Modules.Shipments.Features.Features.Shared.Requests;
using Modules.Shipments.Features.Features.Shared.Routes;
using Address = Modules.Shipments.Domain.ValueObjects.Address;

namespace Modules.Shipments.Features.Features.CreateShipment;

public sealed record CreateShipmentRequest(
    string OrderId,
    Address Address,
    string Carrier,
    string ReceiverEmail,
    List<ShipmentItemRequest> Items);

public class CreateShipmentApiEndpoint : IApiEndpoint
{
    public Asp.Versioning.ApiVersion Version => new(1.0);
    public void MapEndpoint(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
    {
        app.MapPost(RouteConsts.BaseRoute, Handle)
			.RequireAuthorization(ShipmentPolicyConsts.CreatePolicy)
			.WithTags("Shipments")
            .WithDescription("Creates a new shipment.");
    }

    private static async Task<IResult> Handle(
        [FromBody] CreateShipmentRequest request,
        IValidator<CreateShipmentRequest> validator,
        ICreateShipmentHandler handler,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await handler.HandleAsync(request, cancellationToken);
        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return Results.Ok(response.Value);
    }
}
