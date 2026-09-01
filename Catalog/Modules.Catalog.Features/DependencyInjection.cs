using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Modules.Common.API.Abstractions;
using Modules.Common.Application.Extensions;
using Modules.Catalog.PublicApi;
using Modules.Catalog.Infrastructure.Database;
using Modules.Catalog.Features.InternalApi;
using Modules.Catalog.Features.InternalApi.Decorators;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CatalogModuleRegistration
{
	public static string ActivityModuleName => "CatalogModule";
	public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.AddCatalogModuleApi()
			.AddCatalogInfrastructure(configuration);
	}

	private static IServiceCollection AddCatalogModuleApi(this IServiceCollection services)
	{
		services.AddScoped<ProductsModuleApi>();

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
