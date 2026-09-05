using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Checkout.Infrastructure.Database;
using Modules.Common.Infrastructure.Database;

namespace Microsoft.Extensions.DependencyInjection;

public static class CheckoutInfrastructureRegistration
{
	public static IServiceCollection AddCheckoutInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<CheckoutDbContext>(options => options
			.UseNpgsql(configuration.GetConnectionString("Postgres"), npgsql =>
				npgsql.MigrationsHistoryTable("__ef_migrations_history", "checkout"))
			.UseSnakeCaseNamingConvention());
		services.AddScoped<IModuleDatabaseMigrator, CheckoutDatabaseMigrator>();

		return services;
	}
}
