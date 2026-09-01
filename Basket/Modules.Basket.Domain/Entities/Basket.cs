namespace Modules.Basket.Domain.Entities;

public sealed class ShoppingBasket
{
	public ShoppingBasket(string userId, IEnumerable<BasketItem> items)
	{
		UserId = userId;
		Items = items.ToList();
	}

	public string UserId { get; }

	public List<BasketItem> Items { get; }
}

public sealed record BasketItem(
	Guid ProductId,
	string ProductName,
	string? Size,
	string? Color,
	decimal UnitPrice,
	int Quantity);
