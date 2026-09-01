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

namespace Modules.Stocks.Features.Features.IncreaseStock;

public class IncreaseStockApiEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost(RouteConsts.IncreaseStock, Handle)
            .RequireAuthorization(StockPolicyConsts.UpdatePolicy)
			.WithTags(RouteConsts.CommonTag)
			.WithSummary("Increases the stock quantity for a product.")
			.WithDescription("This endpoint allows authorized users to increase the stock quantity for a specified product.");
	}

    private static async Task<IResult> Handle(
        [FromBody] IncreaseStockRequest request,
        IValidator<IncreaseStockRequest> validator,
        [FromServices] IStockModuleApi api,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await api.IncreaseStockAsync(request, cancellationToken);
        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return Results.NoContent();
    }
}
