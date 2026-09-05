using Modules.Common.Domain.Results;

namespace Modules.Checkout.Features.Payments;

internal sealed class FakePaymentGateway : IPaymentGateway
{
	public Task<Result<PaymentAuthorization>> AuthorizeAsync(PaymentRequest request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult<Result<PaymentAuthorization>>(
			new PaymentAuthorization($"fake_{Guid.NewGuid():N}"));
	}
}
