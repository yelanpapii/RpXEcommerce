using Microsoft.Extensions.Logging;
using Modules.Basket.Domain.Repositories;
using Modules.Basket.Features.Features.Shared;
using Modules.Basket.PublicApi.Contracts;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;

namespace Modules.Basket.Features.Features.GetBasket;

internal interface IGetBasketHandler : IHandler
{
	Task<Result<BasketResponse>> HandleAsync(string userId, CancellationToken cancellationToken);
}

internal sealed class GetBasketHandler(
	IBasketStore store,
	ILogger<GetBasketHandler> logger) : IGetBasketHandler
{
	public async Task<Result<BasketResponse>> HandleAsync(string userId, CancellationToken cancellationToken)
	{
		var basket = await store.GetAsync(userId, cancellationToken);
		if (basket is null)
		{
			logger.LogInformation("Basket for user {UserId} was not found", userId);
			return new BasketResponse(userId, [], 0m);
		}

		return basket.ToResponse();
	}
}
