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

namespace Modules.Stocks.Features.Features.GetStocksByProductName;

public class GetStocksByProductNameApiEndpoint : IApiEndpoint
{
    public Asp.Versioning.ApiVersion Version => new(1.0);
    public void MapEndpoint(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
    {
        app.MapGet(RouteConsts.GetStocksByProductName, Handle)
            .RequireAuthorization(StockPolicyConsts.ReadPolicy)
			.WithTags(RouteConsts.CommonTag)
			.WithSummary("Retrieves stock information for a product.")
			.WithDescription("This endpoint allows authorized users to retrieve the stock information for a specified product.");
	}

    private static async Task<IResult> Handle(
        [FromQuery] string productName,
        IValidator<GetStocksByProductNameRequest> validator,
        [FromServices] IStockModuleApi api,
        CancellationToken cancellationToken)
    {
        var request = new GetStocksByProductNameRequest(productName);
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await api.GetStockByProductNameAsync(request, cancellationToken);
        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return Results.Ok(response.Value);
    }
}
