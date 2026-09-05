using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Modules.Checkout.Features.Features.StartCheckout;
using Modules.Checkout.Features.InternalApi;
using Modules.Checkout.Features.Payments;
using Modules.Checkout.Infrastructure;
using Modules.Checkout.PublicApi;
using Modules.Common.API.Abstractions;
using Modules.Common.Application.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class CheckoutModuleRegistration
{
	public static string ActivityModuleName => "CheckoutModule";

	public static IServiceCollection AddCheckoutModule(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<ICheckoutModuleApi, CheckoutModuleApi>();
		services.AddScoped<IPaymentGateway, FakePaymentGateway>();
		services.RegisterApiEndpointsFromAssemblyContaining(typeof(CheckoutModuleRegistration));
		services.RegisterHandlersFromAssemblyContaining(typeof(CheckoutModuleRegistration));
		services.AddValidatorsFromAssembly(typeof(CheckoutModuleRegistration).Assembly);
		services.AddCheckoutInfrastructure(configuration);
		return services;
	}
}
