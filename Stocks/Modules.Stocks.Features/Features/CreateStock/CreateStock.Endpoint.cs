using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Stocks.Domain.Policies;
using Modules.Stocks.Features.Features.Shared.Routes;
using Modules.Stocks.PublicApi;
using Modules.Stocks.PublicApi.Contracts;

namespace Modules.Stocks.Features.Features.CreateStock;

public class CreateStockApiEndpoint : IApiEndpoint
{
    public Asp.Versioning.ApiVersion Version => new(1.0);
    public void MapEndpoint(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
    {
        app.MapPost(RouteConsts.Create, Handle)
            .RequireAuthorization(StockPolicyConsts.CreatePolicy)
			.WithTags(RouteConsts.CommonTag)
			.WithSummary("Creates a new stock entry for a product.")
			.WithDescription("This endpoint allows authorized users to create a new stock entry for a specified product with an initial quantity.");
	}

    private static async Task<IResult> Handle(
        [FromBody] CreateStockRequest request,
        IValidator<CreateStockRequest> validator,
        [FromServices] IStockModuleApi api,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await api.CreateStockAsync(request, cancellationToken);
        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return Results.Ok(response.Value);
    }
}
