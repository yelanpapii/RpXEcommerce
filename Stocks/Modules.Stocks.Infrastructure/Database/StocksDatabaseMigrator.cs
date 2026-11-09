using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Infrastructure.Database;

namespace Modules.Stocks.Infrastructure.Database;

public class StocksDatabaseMigrator : IModuleDatabaseMigrator
{
    public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<StocksDbContext>();

		try
		{
			var conn = dbContext.Database.GetDbConnection().ConnectionString;
			Console.WriteLine($"[StocksDatabaseMigrator] ConnectionString: {conn}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[StocksDatabaseMigrator] Could not read connection string: {ex.Message}");
		}

		try
		{
			var applied = await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken);
			var pending = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);

			Console.WriteLine($"[StocksDatabaseMigrator] Applied migrations: {string.Join(',', applied)}");
			Console.WriteLine($"[StocksDatabaseMigrator] Pending migrations: {string.Join(',', pending)}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[StocksDatabaseMigrator] Could not enumerate migrations: {ex.Message}");
		}

		await dbContext.Database.MigrateAsync(cancellationToken);
	}
}
