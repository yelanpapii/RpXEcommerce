using Modules.Basket.Domain.Entities;

namespace Modules.Basket.Domain.Repositories;

public interface IBasketStore
{
	Task<ShoppingBasket?> GetAsync(string userId, CancellationToken cancellationToken);
	Task SaveAsync(ShoppingBasket basket, CancellationToken cancellationToken);
	Task DeleteAsync(string userId, CancellationToken cancellationToken);
}
