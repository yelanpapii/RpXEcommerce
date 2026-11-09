using System;
using System.Collections.Generic;

namespace Modules.Catalog.Features.Features.Shared.Converters;

/// <summary>
/// Thin, SQL-projectable shape for a product row. It intentionally contains only scalar columns
/// so EF Core can translate the projection to SQL without materializing the <c>Product</c> entity.
/// </summary>
internal sealed record ProductProjection(
	Guid Id,
	string Name,
	string? Description,
	string CategoryCode,
	int StyleId,
	IReadOnlyList<ProductVariantProjection> Variants,
	DateTimeOffset CreatedAt);

/// <summary>
/// Thin, SQL-projectable shape for a single product variant. Color/size are carried as their raw
/// codes (converted scalar columns); their display names are resolved in memory afterwards.
/// </summary>
internal sealed record ProductVariantProjection(
	Guid Id,
	Guid ProductId,
	string Sku,
	string ColorCode,
	string SizeCode,
	decimal Price,
	int Stock);
