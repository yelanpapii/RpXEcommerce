using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Infrastructure.Database;

namespace Modules.Checkout.Infrastructure.Database;

public sealed class CheckoutDatabaseMigrator : IModuleDatabaseMigrator
{
	public async Task MigrateAsync(IServiceScope scope, CancellationToken cancellationToken = default)
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<CheckoutDbContext>();
		await dbContext.Database.MigrateAsync(cancellationToken);
	}
}
