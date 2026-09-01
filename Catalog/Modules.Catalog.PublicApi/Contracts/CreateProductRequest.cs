namespace Modules.Catalog.PublicApi.Contracts;

public record CreateProductRequest(string Name, string? Description, string? SKU, decimal Price);
