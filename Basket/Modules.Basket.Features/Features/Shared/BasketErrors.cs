using Modules.Common.Domain.Results;

namespace Modules.Basket.Features.Features.Shared;

internal static class BasketErrors
{
	internal static Error InvalidUser => Error.Unauthorized("Basket.Unauthorized", "The authenticated user is required.");
	internal static Error ItemNotFound(Guid productId) => Error.NotFound("Basket.ItemNotFound", $"Product {productId} was not found in the basket.");
}
