namespace Modules.Basket.PublicApi.Contracts;

/// <summary>Payload for adding a clothing product to the current basket.</summary>
public sealed record AddBasketItemRequest
{
	public required Guid ProductId { get; init; }
	public required string ProductName { get; init; }
	public string? Size { get; init; }
	public string? Color { get; init; }
	public required decimal UnitPrice { get; init; }
	public required int Quantity { get; init; }
}

/// <summary>Payload for changing the quantity of a product in the current basket.</summary>
public sealed record UpdateBasketItemRequest
{
	public required Guid ProductId { get; init; }
	public string? Size { get; init; }
	public string? Color { get; init; }
	public required int Quantity { get; init; }
}

/// <summary>Represents the current shopping basket.</summary>
public sealed record BasketResponse(
	string UserId,
	IReadOnlyCollection<BasketItemResponse> Items,
	decimal Total);

/// <summary>Represents a clothing product line in a basket.</summary>
public sealed record BasketItemResponse(
	Guid ProductId,
	string ProductName,
	string? Size,
	string? Color,
	decimal UnitPrice,
	int Quantity,
	decimal Subtotal);
