namespace Modules.Catalog.PublicApi.Contracts;

public record ProductDto(Guid Id, string Name, string? Description, string? SKU, decimal Price, DateTimeOffset CreatedAt);
