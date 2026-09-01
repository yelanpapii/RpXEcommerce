using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Domain.Entities;

namespace Modules.Catalog.Infrastructure.Database;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> optionsBuilder) : DbContext(optionsBuilder)
{
	public DbSet<Product> Products { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.HasDefaultSchema(DbConsts.catalogSchemaName);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
	}
}
