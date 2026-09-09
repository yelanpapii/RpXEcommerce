namespace Modules.Catalog.PublicApi.Contracts;

public sealed record ProductDto(
	Guid Id,
	string Name,
	string? Description,
	string CategoryCode,
	int StyleId,
	List<ProductVariantDto> Variants,
	DateTimeOffset CreatedAt);

public sealed record ProductVariantDto(
	Guid Id,
	string SKU,
	string ColorName,
	string ColorCode,
	string Size,
	decimal Price,
	int Stock);
