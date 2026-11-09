using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Modules.Stocks.Infrastructure.Database;

public class StocksDbContextFactory : IDesignTimeDbContextFactory<StocksDbContext>
{
	public StocksDbContext CreateDbContext(string[] args)
	{
		var basePath = Directory.GetCurrentDirectory();
		var config = new ConfigurationBuilder()
			.SetBasePath(basePath)
			.AddJsonFile("appsettings.json", optional: true)
			.AddEnvironmentVariables()
			.Build();

		var conn = config.GetConnectionString("Postgres");

		var builder = new DbContextOptionsBuilder<StocksDbContext>();
		builder.UseNpgsql(conn, b => b.MigrationsAssembly(typeof(StocksDbContext).Assembly.FullName))
			.UseSnakeCaseNamingConvention();

		return new StocksDbContext(builder.Options);
	}
}
