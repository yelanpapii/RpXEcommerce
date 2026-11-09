using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Catalog.Domain.Entities;
using Modules.Catalog.Domain.ValueObjects;

namespace Modules.Catalog.Infrastructure.Database.Mapping;

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
	public void Configure(EntityTypeBuilder<ProductVariant> builder)
	{
		builder.ToTable("catalog_product_variants");
		builder.HasKey(variant => variant.Id);

		builder.Property(v => v.Sku)
			.HasConversion(
				sku => sku.ToString(),
				value => ClothingSku.FromPersistedValue(value))
			.HasColumnName("sku")
			.HasMaxLength(40)
			.IsRequired();

		builder.HasIndex(v => v.Sku).IsUnique();

		builder.Property(v => v.Color)
			.HasConversion(
				color => color.Code,
				code => Color.Of(code))
			.HasColumnName("color_code")
			.HasMaxLength(6)
			.IsRequired();

		builder.Property(v => v.Size)
			.HasConversion(
				size => size.Code,
				code => Size.Of(code))
			.HasColumnName("size")
			.HasMaxLength(10)
			.IsRequired();

		builder.Property(v => v.Price)
			.HasColumnName("price")
			.HasPrecision(18, 2)
			.IsRequired();

		builder.Property(v => v.Stock)
			.HasColumnName("stock")
			.IsRequired();

		builder.Property(v => v.RowVersion)
			.IsRowVersion();
	}
}
