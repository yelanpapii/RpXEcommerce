using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Checkout.Domain.Entities;
using CheckoutEntity = Modules.Checkout.Domain.Entities.Checkout;

namespace Modules.Checkout.Infrastructure.Database.Mapping;

internal sealed class CheckoutConfiguration : IEntityTypeConfiguration<CheckoutEntity>
{
	public void Configure(EntityTypeBuilder<CheckoutEntity> builder)
	{
		builder.HasKey(x => x.Id);
		builder.HasIndex(x => x.OrderId).IsUnique();
		builder.Property(x => x.UserId).IsRequired();
		builder.Property(x => x.OrderId).IsRequired();
		builder.Property(x => x.ShipmentNumber).IsRequired(false);
		builder.Property(x => x.Total).HasPrecision(18, 2).IsRequired();
		builder.Property(x => x.Status).HasConversion<string>().IsRequired();
	}
}
