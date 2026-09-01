using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Infrastructure.Database;

namespace Modules.Catalog.Infrastructure.Database;

public class CatalogDatabaseMigrator : IModuleDatabaseMigrator
{
	public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default)
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
		await dbContext.Database.MigrateAsync(cancellationToken);
	}
}
