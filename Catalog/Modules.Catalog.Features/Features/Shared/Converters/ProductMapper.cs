using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Modules.Catalog.Domain.Entities;
using Modules.Catalog.Domain.Lookups.Interfaces;
using Modules.Catalog.Domain.ValueObjects;
using Modules.Catalog.Features.Features.Shared.Errors;
using Modules.Catalog.PublicApi.Contracts;
using Modules.Common.Domain.Results;

namespace Modules.Catalog.Features.Features.Shared.Converters;

internal sealed class ProductMapper
{
	private readonly IColorCatalog _colorCatalog;
	private readonly ISizeCatalog _sizeCatalog;

	public ProductMapper(IColorCatalog colorCatalog, ISizeCatalog sizeCatalog)
	{
		_colorCatalog = colorCatalog;
		_sizeCatalog = sizeCatalog;
	}

	public ProductDto ToDto(Product product) =>
	   new(
		   product.Id,
		   product.Name,
		   product.Description,
		   product.CategoryCode.Value,
		   product.StyleId,
		   product.Variants.Select(ToDto).ToList(),
		   product.CreatedAt);

	public ProductDto ToDto(ProductProjection projection) =>
		new(
			projection.Id,
			projection.Name,
			projection.Description,
			projection.CategoryCode,
			projection.StyleId,
			projection.Variants.Select(ToDto).ToList(),
			projection.CreatedAt);

	private ProductVariantDto ToDto(ProductVariantProjection variant) =>
		new(
			variant.Id,
			variant.ProductId,
			variant.Sku,
			variant.ColorCode,
			_colorCatalog.GetDisplayName(variant.ColorCode),
			variant.SizeCode,
			_sizeCatalog.GetDisplayName(variant.SizeCode),
			variant.Price,
			variant.Stock);

	private ProductVariantDto ToDto(Domain.Entities.ProductVariant variant) =>
		new(
			variant.Id,
			variant.ProductId,
			variant.Sku.ToString(),
			variant.Color.Code,
			_colorCatalog.GetDisplayName(variant.Color),
			variant.Size.Code,
			_sizeCatalog.GetDisplayName(variant.Size),
			variant.Price,
			variant.Stock);

	/// <summary>
	/// Maps a <see cref="CreateProductRequest"/> to a new <see cref="Product"/> aggregate.
	/// This is the write/validation path: color and size codes are validated against the
	/// configured catalog and a validation <see cref="Error"/> is returned (rather than thrown)
	/// when a code is missing or unknown.
	/// </summary>
	/// <param name="request">The product creation request to map.</param>
	/// <returns>A successful <see cref="Product"/>, or a validation error when a color/size code is invalid.</returns>
	public Result<Product> MapToProduct(CreateProductRequest request)
	{
		var product = Product.Create(request.Name, request.Description, CategoryCode.Create(request.CategoryCode), int.Parse(request.StyleId, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));

		foreach (var productVariant in request.ProductVariant ?? [])
		{
			var sizeFromCatalog = _sizeCatalog.Resolve(productVariant.Size);
			if (sizeFromCatalog is null)
			{
				return CatalogErrors.ValidationError(
					nameof(productVariant.Size),
					$"The size '{productVariant.Size}' is not configured in the product catalog.");
			}

			var colorFromCatalog = _colorCatalog.Resolve(productVariant.ColorName);
			if (colorFromCatalog is null)
			{
				return CatalogErrors.ValidationError(
					nameof(productVariant.ColorName),
					$"The color '{productVariant.ColorName}' is not configured in the product catalog.");
			}

			product.AddVariant(colorFromCatalog, sizeFromCatalog, productVariant.Price, productVariant.Stock);
		}

		return product;
	}
}
