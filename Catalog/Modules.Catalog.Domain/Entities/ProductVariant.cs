using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Modules.Catalog.Domain.ValueObjects;

namespace Modules.Catalog.Domain.Entities;

public sealed class ProductVariant
{
	public Guid Id { get; private set; }
	public Guid ProductId { get; private set; }
	public Product Product { get; private set; } = null!;

	public ClothingSku Sku { get; private set; }
	public Color Color { get; private set; } = null!;
	public Size Size { get; private set; } = null!;
	public decimal Price { get; private set; }
	public int Stock { get; private set; }

	public byte[] RowVersion { get; } = null!;

	private ProductVariant()
	{
		// EF Core.
	}

	internal static ProductVariant Create(Product product, Color color, Size size, decimal price, int initialStock)
	{
		if (price < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(price), "The price cannot be negative.");
		}

		if (initialStock < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(initialStock), "The initial stock cannot be negative.");
		}

		return new ProductVariant
		{
			Id = Guid.NewGuid(),
			ProductId = product.Id,
			Product = product,
			Sku = new ClothingSku(product.CategoryCode, product.StyleId, color.Code, size.Code),
			Color = color,
			Size = size,
			Price = price,
			Stock = initialStock,
		};
	}
}
