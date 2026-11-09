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
	Guid ProductId,
	string SKU,
	string ColorCode,
	string ColorName,
	string SizeCode,
	string SizeName,
	decimal Price,
	int Stock);
