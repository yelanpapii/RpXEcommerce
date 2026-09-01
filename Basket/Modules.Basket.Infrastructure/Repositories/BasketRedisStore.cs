using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Modules.Basket.Domain.Entities;
using Modules.Basket.Domain.Repositories;

namespace Modules.Basket.Infrastructure.Repositories;

internal sealed class BasketRedisStore(IDistributedCache cache) : IBasketStore
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	public async Task<ShoppingBasket?> GetAsync(string userId, CancellationToken cancellationToken)
	{
		var json = await cache.GetStringAsync(GetKey(userId), cancellationToken);
		return json is null ? null : JsonSerializer.Deserialize<ShoppingBasket>(json, JsonOptions);
	}

	public Task SaveAsync(ShoppingBasket basket, CancellationToken cancellationToken)
	{
		var json = JsonSerializer.Serialize(basket, JsonOptions);
		return cache.SetStringAsync(GetKey(basket.UserId), json, cancellationToken);
	}

	public Task DeleteAsync(string userId, CancellationToken cancellationToken) =>
		cache.RemoveAsync(GetKey(userId), cancellationToken);

	private static string GetKey(string userId) => $"basket:{userId}";
}
