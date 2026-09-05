using Modules.Checkout.Domain.Enums;
using Modules.Common.Domain.Results;

namespace Modules.Checkout.PublicApi.Contracts;

public sealed record StartCheckoutRequest(string Carrier);

public sealed record CheckoutResponse(
	Guid Id,
	string OrderId,
	string ShipmentNumber,
	decimal Total,
	CheckoutStatus Status,
	DateTimeOffset CreatedAt);
