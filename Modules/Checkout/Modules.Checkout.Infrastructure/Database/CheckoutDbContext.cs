using Microsoft.EntityFrameworkCore;
using Modules.Checkout.Domain.Entities;
using CheckoutEntity = Modules.Checkout.Domain.Entities.Checkout;

namespace Modules.Checkout.Infrastructure.Database;

public sealed class CheckoutDbContext(DbContextOptions<CheckoutDbContext> options) : DbContext(options)
{
	public DbSet<CheckoutEntity> Checkouts => Set<CheckoutEntity>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("checkout");
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(CheckoutDbContext).Assembly);
	}
}
