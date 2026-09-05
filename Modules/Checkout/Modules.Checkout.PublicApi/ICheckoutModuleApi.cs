using Modules.Checkout.PublicApi.Contracts;
using Modules.Common.Domain.Results;

namespace Modules.Checkout.PublicApi;

public interface ICheckoutModuleApi
{
	Task<Result<CheckoutResponse>> StartAsync(string userId, StartCheckoutRequest request, CancellationToken cancellationToken = default);
}
