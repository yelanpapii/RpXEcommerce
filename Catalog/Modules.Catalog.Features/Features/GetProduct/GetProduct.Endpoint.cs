using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Catalog.Domain.Policies;
using Modules.Catalog.Features.Features.Shared.Routes;
using Modules.Catalog.PublicApi;
using Modules.Common.API.Abstractions;

namespace Modules.Catalog.Features.Features.GetProduct;

public class GetProduct : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapGet(RouteConsts.GetProductById, Handle)
			.RequireAuthorization(CatalogPolicyConsts.ReadPolicy)
			.WithTags(RouteConsts.CommonTag)
			.WithSummary("Retrieves information for a specific product.")
			.WithDescription("This endpoint allows authorized users to retrieve information for a specified product.");
	}

	private static async Task<IResult> Handle(
		[FromServices] IProductsModuleApi api,
		string productId)
	{
		var result = await api.GetProductByIdAsync(productId);
		if (!result.IsSuccess)
		{
			return Results.NotFound();
		}

		return Results.Ok(result.Value);
	}
}
