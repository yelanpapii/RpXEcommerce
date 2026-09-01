using Modules.Basket.Domain.Entities;
using Modules.Basket.PublicApi.Contracts;

namespace Modules.Basket.Features.Features.Shared;

internal static class BasketMapping
{
	internal static BasketResponse ToResponse(this ShoppingBasket basket) =>
		new(basket.UserId, basket.Items.Select(ToResponse).ToArray(), basket.Items.Sum(item => item.UnitPrice * item.Quantity));

	private static BasketItemResponse ToResponse(BasketItem item) =>
		new(item.ProductId, item.ProductName, item.Size, item.Color, item.UnitPrice, item.Quantity, item.UnitPrice * item.Quantity);
}
