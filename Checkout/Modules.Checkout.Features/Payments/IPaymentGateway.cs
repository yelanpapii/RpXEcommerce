using Modules.Common.Domain.Results;

namespace Modules.Checkout.Features.Payments;

public sealed record PaymentAuthorization(string TransactionId);

public sealed record PaymentRequest(string OrderId, string UserId, decimal Amount);

public interface IPaymentGateway
{
	Task<Result<PaymentAuthorization>> AuthorizeAsync(PaymentRequest request, CancellationToken cancellationToken = default);
}
