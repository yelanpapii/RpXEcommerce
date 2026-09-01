using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Basket.Domain.Repositories;
using Modules.Basket.Infrastructure.Policies;
using Modules.Basket.Infrastructure.Repositories;
using Modules.Common.Infrastructure.Policies;

namespace Modules.Basket.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddBasketInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("RedisConnection")
			?? throw new InvalidOperationException("Redis connection string is not configured");

		services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);
		services.AddScoped<IBasketStore, BasketRedisStore>();
		services.AddSingleton<IPolicyFactory, BasketPolicyFactory>();

		return services;
	}
}
