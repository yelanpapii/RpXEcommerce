using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Modules.Catalog.Domain.Entities;
using Modules.Catalog.PublicApi.Contracts;

namespace Modules.Catalog.Features.Features.CreateProduct;

internal static class CreateProductMappingExtensions
{
	public static Product MapToProduct(this CreateProductRequest request)
	{
		return new Product
		{
			Id = Guid.CreateVersion7(),
			Name = request.Name,
			Description = request.Description,
			Price = request.Price,
		};
	}
}
