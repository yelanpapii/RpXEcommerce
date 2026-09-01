using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Modules.Basket.Features.InternalApi;
using Modules.Basket.Features.InternalApi.Decorators;
using Modules.Basket.Infrastructure;
using Modules.Basket.PublicApi;
using Modules.Common.API.Abstractions;
using Modules.Common.Application.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class BasketModuleRegistration
{
	public static string ActivityModuleName => "BasketModule";

	public static IServiceCollection AddBasketModule(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<BasketModuleApi>();

		services.AddScoped<IBasketModuleApi>(provider =>
		{
			var actualImplementation = provider.GetRequiredService<BasketModuleApi>();
			return new TracedBasketModuleApi(actualImplementation);
		});

		services.RegisterApiEndpointsFromAssemblyContaining(typeof(BasketModuleRegistration));
		services.RegisterHandlersFromAssemblyContaining(typeof(BasketModuleRegistration));
		services.AddValidatorsFromAssembly(typeof(BasketModuleRegistration).Assembly);

		services.AddBasketInfrastructure(configuration);

		return services;
	}
}

public sealed class BasketMiddlewareConfigurator : IModuleMiddlewareConfigurator
{
	// no special middleware yet
	public IApplicationBuilder Configure(IApplicationBuilder app) => app;
}
