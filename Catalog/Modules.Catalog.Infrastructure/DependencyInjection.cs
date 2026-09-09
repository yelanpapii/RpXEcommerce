using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Catalog.Infrastructure.Database;
using Modules.Catalog.Infrastructure.Policies;
using Modules.Catalog.Infrastructure.Messaging;
using Modules.Common.Infrastructure.Database;
using Modules.Common.Infrastructure.Policies;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
	public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		var postgresConnectionString = configuration.GetConnectionString("Postgres");

		services.AddDbContext<CatalogDbContext>(x => x
			.UseNpgsql(postgresConnectionString, npgsqlOptions =>
				npgsqlOptions.MigrationsHistoryTable(DbConsts.MigrationHistoryTableName, DbConsts.catalogSchemaName))
			.UseSnakeCaseNamingConvention()
		);

		services.AddScoped<IModuleDatabaseMigrator, CatalogDatabaseMigrator>();
		services.AddSingleton<IPolicyFactory, CatalogPolicyFactory>();
		services.AddHostedService<CatalogOutboxDispatcher>();

		return services;
	}
}
