using Modules.Catalog.Domain.ValueObjects;

namespace Modules.Catalog.Domain.Entities;

public class Product
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;
	public string? Description { get; set; }
	public string CategoryCode { get; set; } = null!; // Ej: "TSHR", "JEAN"
	public int StyleId { get; set; }                  // Ej: 104 (se guardará como 0104)

	public DateTimeOffset CreatedAt { get; set; }

	public List<ProductVariant> Variants { get; set; } = new();
}

public class ProductVariant
{
	public Guid Id { get; set; }
	public Guid ProductId { get; set; }
	public Product Product { get; set; } = null!;
	public ClothingSku Sku { get; set; }
	public string ColorName { get; set; } = null!;  // Ej: "Azul Marino"
	public string ColorCode { get; set; } = null!;  // Ej: "001"
	public string SizeName { get; set; } = null!;   // Ej: "Medium"
	public decimal Price { get; set; }
	public int Stock { get; set; }
}
