using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using Modules.Catalog.Domain.Entities;
using Modules.Catalog.Domain.ValueObjects;
using Modules.Catalog.PublicApi.Contracts;

namespace Modules.Catalog.Features.Features.CreateProduct;

internal static class CreateProductMappingExtensions
{
	public static Product MapToProduct(this CreateProductRequest request)
	{
		var product = new Product
		{
			Id = Guid.CreateVersion7(),
			Name = request.Name,
			CategoryCode = request.CategoryCode,
			StyleId = int.Parse(request.StyleId, System.Globalization.NumberStyles.Integer),
			Description = request.Description,
			CreatedAt = DateTimeOffset.UtcNow
		};

		foreach (var productVariant in request.ProductVariant ?? [])
		{
			product.Variants.Add(new Domain.Entities.ProductVariant
			{
				Id = Guid.NewGuid(),
				ColorName = productVariant.ColorName,
				ColorCode = productVariant.ColorCode,
				SizeName = productVariant.Size,
				Price = productVariant.Price,
				Stock = productVariant.Stock,
				Sku = new ClothingSku(product.CategoryCode, product.StyleId, productVariant.ColorCode, productVariant.Size)
			});
		}

		return product;
		
	}
}
