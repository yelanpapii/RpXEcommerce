using FluentValidation;
using Microsoft.Extensions.Logging;
using Modules.Basket.Features.Features.Shared;
using Modules.Basket.PublicApi.Contracts;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Basket.Features.Features.Shared.Errors;
using Modules.Basket.Infrastructure.Repositories;
using Modules.Basket.Domain.Repositories;

namespace Modules.Basket.Features.Features.UpdateBasketItem;

internal interface IUpdateBasketItemHandler : IHandler
{
	Task<Result<BasketResponse>> HandleAsync(string userId, UpdateBasketItemRequest request, CancellationToken cancellationToken);
}

internal sealed class UpdateBasketItemHandler(
	IBasketStore store,
	IValidator<UpdateBasketItemRequest> validator,
	ILogger<UpdateBasketItemHandler> logger) : IUpdateBasketItemHandler
{
	public async Task<Result<BasketResponse>> HandleAsync(string userId, UpdateBasketItemRequest request, CancellationToken cancellationToken)
	{
		var validation = await validator.ValidateAsync(request, cancellationToken);
		if (!validation.IsValid)
		{
			return validation.ToDomainErrors();
		}

		var basket = await store.GetAsync(userId, cancellationToken);
		if (basket is null)
		{
			return BasketErrors.ItemNotFound(request.ProductId);
		}

		var index = basket.Items.FindIndex(item => item.ProductId == request.ProductId && item.Size == request.Size && item.Color == request.Color);
		if (index < 0)
		{
			return BasketErrors.ItemNotFound(request.ProductId);
		}

		basket.Items[index] = basket.Items[index] with { Quantity = request.Quantity };
		await store.SaveAsync(basket, cancellationToken);
		logger.LogInformation("Updated product {ProductId} in basket for user {UserId}", request.ProductId, userId);
		return basket.ToResponse();
	}
}
