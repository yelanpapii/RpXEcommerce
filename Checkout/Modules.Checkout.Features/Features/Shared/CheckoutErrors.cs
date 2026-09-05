using Modules.Common.Domain.Results;

namespace Modules.Checkout.Features.Features.Shared;

internal static class CheckoutErrors
{
	public static Error EmptyBasket() => Error.Validation("Checkout.EmptyBasket", "The basket is empty.");
	public static Error PriceChanged(Guid productId) => Error.Conflict("Checkout.PriceChanged", $"The price for product '{productId}' has changed.");
	public static Error ProductNotFound(Guid productId) => Error.NotFound("Checkout.ProductNotFound", $"Product '{productId}' was not found.");
	public static Error AddressMissing() => Error.Validation("Checkout.AddressMissing", "The user's shipping address is incomplete.");
	public static Error AlreadyExists(string userId) => Error.Conflict("Checkout.AlreadyExists", $"A checkout already exists for user '{userId}'.");
}
