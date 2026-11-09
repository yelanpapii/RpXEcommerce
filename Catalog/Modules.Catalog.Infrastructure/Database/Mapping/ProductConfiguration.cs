using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Catalog.Domain.Entities;

namespace Modules.Catalog.Infrastructure.Database.Mapping;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.ToTable("catalog_products");
		builder.HasKey(x => x.Id);

		builder.Property(p => p.Name)
		   .HasMaxLength(200)
		   .IsRequired();

		builder.Property(p => p.Description)
			.HasMaxLength(2000);

		builder.Property(p => p.CategoryCode)
			.HasConversion(
				code => code.Value,
				value => CategoryCode.Create(value))
			.HasColumnName("category_code")
			.HasMaxLength(8)
			.IsRequired();

		builder.Property(p => p.StyleId)
			.HasColumnName("style_id")
			.IsRequired();

		builder.Property(p => p.Status)
			.HasColumnName("status")
			.HasConversion<string>()
			.HasMaxLength(20)
			.IsRequired();

		builder.Property(p => p.CreatedAt)
			.HasColumnName("created_at")
			.IsRequired();

		builder.Property(p => p.UpdatedAt)
			.HasColumnName("updated_at")
			.IsRequired();

		builder.HasMany(p => p.Variants)
			.WithOne(v => v.Product)
			.HasForeignKey(v => v.ProductId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Metadata
			.FindNavigation(nameof(Product.Variants))!
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.HasIndex(p => new { p.CategoryCode, p.StyleId }).IsUnique();
	}
}
