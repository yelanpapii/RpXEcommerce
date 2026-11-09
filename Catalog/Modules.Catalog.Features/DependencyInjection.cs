using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Modules.Catalog.Features.Caching;
using Microsoft.Extensions.Configuration;
using Modules.Catalog.Domain.Lookups.Interfaces;
using Modules.Catalog.Domain.Options;
using Modules.Catalog.Features.Features.Shared.Converters;
using Modules.Catalog.Features.InternalApi;
using Modules.Catalog.Features.InternalApi.Decorators;
using Modules.Catalog.Infrastructure.Database;
using Modules.Catalog.Infrastructure.Lookups;
using Modules.Catalog.PublicApi;
using Modules.Common.API.Abstractions;
using Modules.Common.Application.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CatalogModuleRegistration
{
	public static string ActivityModuleName => "CatalogModule";
	public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.AddCatalogModuleApi(configuration)
			.AddCatalogInfrastructure(configuration);
	}

	private static IServiceCollection AddCatalogModuleApi(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddStackExchangeRedisOutputCache(options =>
		{
			options.Configuration = configuration.GetConnectionString("RedisConnection");

		});

		services.AddOutputCache(options =>
		{
			options.AddPolicy("ShortTTL", policy => policy
				.Expire(TimeSpan.FromSeconds(30))
				.SetVaryByQuery("page", "pageSize")
				.AddPolicy<AuthenticatedOutputCachePolicy>());
			options.AddPolicy("LongTTL", policy => policy.Expire(TimeSpan.FromHours(1)));
		});

		services.Configure<ProductCatalogOptions>(configuration.GetSection(ProductCatalogOptions.SectionName));

		services.AddScoped<IColorCatalog, ConfigurationColorCatalog>();
		services.AddScoped<ISizeCatalog, ConfigurationSizeCatalog>();
		services.AddScoped<ProductsModuleApi>();
		services.AddScoped<ProductMapper>();

		services.AddScoped<IProductsModuleApi>(provider =>
		{
			var actualImplementation = provider.GetRequiredService<ProductsModuleApi>();
			return new TracedProductsModuleApi(actualImplementation);
		});

		services.RegisterApiEndpointsFromAssemblyContaining(typeof(CatalogModuleRegistration));
		services.RegisterHandlersFromAssemblyContaining(typeof(CatalogModuleRegistration));
		services.AddValidatorsFromAssembly(typeof(CatalogModuleRegistration).Assembly);

		return services;
	}
}

public class CatalogMiddlewareConfigurator : IModuleMiddlewareConfigurator
{
	public IApplicationBuilder Configure(IApplicationBuilder app)
	{
		return app; // no special middleware yet
	}
}
