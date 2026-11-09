using Modules.Catalog.Domain.ValueObjects;

namespace Modules.Catalog.Domain.Entities;

public sealed class Product
{
	public Guid Id { get; private set; }
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public CategoryCode CategoryCode { get; private set; } = null!;
	public int StyleId { get; private set; }
	public ProductStatus Status { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	private readonly List<ProductVariant> _variants = new();
	public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

	private Product()
	{
		// EF Core.
	}

	public static Product Create(string name, string? description, CategoryCode categoryCode, int styleId)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("The product name is required.", nameof(name));
		}

		if (styleId <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(styleId), "The style ID must be a positive integer.");
		}

		var now = DateTimeOffset.UtcNow;

		return new Product
		{
			Id = Guid.NewGuid(),
			Name = name.Trim(),
			Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
			CategoryCode = categoryCode,
			StyleId = styleId,
			Status = ProductStatus.Active,
			CreatedAt = now,
			UpdatedAt = now,
		};
	}

	public ProductVariant AddVariant(Color color, Size size, decimal price, int initialStock)
	{
		if (_variants.Any(v => v.Color.Code == color.Code && v.Size == size))
		{
			throw new InvalidOperationException($"A variant for color '{color.Code}' and size '{size}' already exists.");
		}

		var variant = ProductVariant.Create(this, color, size, price, initialStock);
		_variants.Add(variant);
		Touch();
		return variant;
	}

	public void Rename(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("The product name is required.", nameof(name));
		}

		Name = name.Trim();
		Touch();
	}

	public void Discontinue()
	{
		Status = ProductStatus.Discontinued;
		Touch();
	}

	public void Reactivate()
	{
		Status = ProductStatus.Active;
		Touch();
	}

	private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
