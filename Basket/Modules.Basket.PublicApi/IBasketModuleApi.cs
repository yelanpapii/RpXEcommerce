using Modules.Basket.PublicApi.Contracts;
using Modules.Common.Domain.Results;

namespace Modules.Basket.PublicApi;

public interface IBasketModuleApi
{
	Task<Result<BasketResponse>> AddItemAsync(string userId, AddBasketItemRequest request, CancellationToken cancellationToken = default);
	Task<Result<BasketResponse>> UpdateItemAsync(string userId, UpdateBasketItemRequest request, CancellationToken cancellationToken = default);
	Task<Result<Success>> DeleteAsync(string userId, CancellationToken cancellationToken = default);
}
