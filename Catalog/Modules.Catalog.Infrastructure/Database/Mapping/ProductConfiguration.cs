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
		builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
		builder.Property(x => x.Description).HasMaxLength(1000);
		builder.Property(x => x.SKU).HasMaxLength(100);
		builder.Property(x => x.Price).HasPrecision(18, 2);
		builder.Property(x => x.CreatedAt).IsRequired();
	}
}
