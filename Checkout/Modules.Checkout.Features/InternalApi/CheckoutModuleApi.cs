using Modules.Checkout.Features.Features.StartCheckout;
using Modules.Checkout.PublicApi;
using Modules.Checkout.PublicApi.Contracts;
using Modules.Common.Domain.Results;

namespace Modules.Checkout.Features.InternalApi;

internal sealed class CheckoutModuleApi(IStartCheckoutHandler startCheckoutHandler) : ICheckoutModuleApi
{
	public async Task<Result<CheckoutResponse>> StartAsync(string userId, StartCheckoutRequest request, CancellationToken cancellationToken = default)
	{
		return await startCheckoutHandler.HandleAsync(userId, request, cancellationToken);
	}
}
