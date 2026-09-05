using System;
using System.Collections.Generic;
using System.Text;
using Modules.Basket.Features.Features.AddBasketItem;
using Modules.Basket.Features.Features.DeleteBasket;
using Modules.Basket.Features.Features.UpdateBasketItem;
using Modules.Basket.Features.Features.GetBasket;
using Modules.Basket.PublicApi;
using Modules.Basket.PublicApi.Contracts;
using Modules.Common.Domain.Results;

namespace Modules.Basket.Features.InternalApi;

internal sealed class BasketModuleApi(
	IGetBasketHandler getBasketHandler,
	IAddBasketItemHandler addBasketItemHandler,
	IDeleteBasketHandler deleteBasketHandler,
	IUpdateBasketItemHandler updateBasketItemHandler) : IBasketModuleApi
{
	public async Task<Result<BasketResponse>> GetAsync(string userId, CancellationToken cancellationToken = default)
	{
		return await getBasketHandler.HandleAsync(userId, cancellationToken);
	}

	public async Task<Result<BasketResponse>> AddItemAsync(string userId, AddBasketItemRequest request, CancellationToken cancellationToken = default)
	{
		return await addBasketItemHandler.HandleAsync(userId, request, cancellationToken);
	}

	public async Task<Result<Success>> DeleteAsync(string userId, CancellationToken cancellationToken = default)
	{
		return await deleteBasketHandler.HandleAsync(userId, cancellationToken);
	}

	public async Task<Result<BasketResponse>> UpdateItemAsync(string userId, UpdateBasketItemRequest request, CancellationToken cancellationToken = default)
	{
		return await updateBasketItemHandler.HandleAsync(userId, request, cancellationToken);
	}
}
