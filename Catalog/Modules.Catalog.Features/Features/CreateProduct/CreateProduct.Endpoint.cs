using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Catalog.Domain.Policies;
using Modules.Catalog.Features.Features.Shared.Routes;
using Modules.Catalog.PublicApi;
using Modules.Catalog.PublicApi.Contracts;
using Modules.Common.API.Abstractions;

namespace Modules.Catalog.Features.Features.CreateProduct;

public class CreateProduct : IApiEndpoint
{
	public Asp.Versioning.ApiVersion Version => new(1.0);
	public void MapEndpoint(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
	{
		app.MapPost(RouteConsts.CreateProduct, Handle)
			.RequireAuthorization(CatalogPolicyConsts.CreatePolicy)
			.WithTags(RouteConsts.CommonTag)
			.WithSummary("Creates a new product.")
			.WithDescription("This endpoint allows authorized users to create a new product with the specified details.");
	}

	private static async Task<IResult> Handle(
		[FromServices] IProductsModuleApi api,
		[FromBody] CreateProductRequest request)
	{
		var result = await api.CreateProductAsync(request);
		if (!result.IsSuccess)
		{
			return Results.BadRequest();
		}
		return Results.Ok(result.Value);
	}
}
