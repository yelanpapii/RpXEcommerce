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
		builder.Property(variant => variant.Sku)
			.HasConversion(
				sku => sku.ToString(),
				value => ClothingSku.Parse(value))
			.HasMaxLength(100)
			.IsRequired();
		builder.Property(variant => variant.ColorName).IsRequired().HasMaxLength(100);
		builder.Property(variant => variant.ColorCode).IsRequired().HasMaxLength(3);
		builder.Property(variant => variant.SizeName).IsRequired().HasMaxLength(20);
		builder.Property(variant => variant.Price).HasPrecision(18, 2).IsRequired();
		builder.Property(variant => variant.Stock).IsRequired();
		builder.HasIndex(variant => variant.Sku).IsUnique();
		builder.HasOne(variant => variant.Product)
			.WithMany(product => product.Variants)
			.HasForeignKey(variant => variant.ProductId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
