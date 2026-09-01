namespace Modules.Catalog.Domain.Entities;

public class Product
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;
	public string? Description { get; set; }
	public string? SKU { get; set; }
	public decimal Price { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
}
