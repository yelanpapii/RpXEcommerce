using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Catalog.Domain.Policies;
using Modules.Catalog.Features.Features.Shared.Routes;
using Modules.Catalog.PublicApi;
using Modules.Common.API.Abstractions;
using Modules.Common.Domain.Results;

namespace Modules.Catalog.Features.Features.GetAllProducts;

public class GetAllProductsEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapGet(RouteConsts.BaseRoute, Handle)
			.WithTags(RouteConsts.CommonTag)
			.RequireAuthorization(CatalogPolicyConsts.ReadPolicy)
			.WithSummary("Retrieves all products.")
			.WithDescription("This endpoint allows authorized users to retrieve a list of all products.");
	}

	private static async Task<Microsoft.AspNetCore.Http.IResult> Handle(
		[FromServices] IProductsModuleApi api,
		CancellationToken cancellationToken,
		[FromQuery(Name = "page")] int page = 1,
		[FromQuery(Name = "pageSize")] int pageSize = 10)
	{
		var result = await api.GetProductsAsync(page, pageSize, cancellationToken);
		if (!result.IsSuccess)
		{
			return Results.UnprocessableEntity();
		}
		return Results.Ok(result.Value);
	}
}
