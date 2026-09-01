using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Catalog.Features.Features.Shared.Routes;

internal static class RouteConsts
{
	internal const string CommonTag = "Catalog";
	internal const string BaseRoute = "/api/products";

	internal const string CreateProduct = BaseRoute;
	internal const string GetProductById = $"{BaseRoute}/{{productId}}";
}

