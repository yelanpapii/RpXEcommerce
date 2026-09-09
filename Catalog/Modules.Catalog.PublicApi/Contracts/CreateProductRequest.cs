namespace Modules.Catalog.PublicApi.Contracts;

public record CreateProductRequest(string Name, string CategoryCode, string StyleId, string? Description, List<ProductVariant>? ProductVariant);

public record struct ProductVariant(string Size, string ColorName, string ColorCode, decimal Price, int Stock);
