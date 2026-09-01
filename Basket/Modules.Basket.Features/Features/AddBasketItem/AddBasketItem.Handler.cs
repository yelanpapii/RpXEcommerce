using FluentValidation;
using Microsoft.Extensions.Logging;
using Modules.Basket.Domain;
using Modules.Basket.Features.Features.Shared;
using Modules.Basket.PublicApi.Contracts;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Basket.Features.Features.Shared.Errors;
using Modules.Basket.Infrastructure.Repositories;
using Modules.Basket.Domain.Repositories;
using Modules.Basket.Domain.Entities;

namespace Modules.Basket.Features.Features.AddBasketItem;

internal interface IAddBasketItemHandler : IHandler
{
	Task<Result<BasketResponse>> HandleAsync(string userId, AddBasketItemRequest request, CancellationToken cancellationToken);
}

internal sealed class AddBasketItemHandler(
	IBasketStore store,
	IValidator<AddBasketItemRequest> validator,
	ILogger<AddBasketItemHandler> logger) : IAddBasketItemHandler
{
	public async Task<Result<BasketResponse>> HandleAsync(string userId, AddBasketItemRequest request, CancellationToken cancellationToken)
	{
		var validation = await validator.ValidateAsync(request, cancellationToken);
		if (!validation.IsValid)
		{
			return validation.ToDomainErrors();
		}

		var basket = await store.GetAsync(userId, cancellationToken) ?? new ShoppingBasket(userId, []);
		var item = basket.Items.FirstOrDefault(existing => existing.ProductId == request.ProductId && existing.Size == request.Size && existing.Color == request.Color);

		if (item is null)
		{
			basket.Items.Add(new BasketItem(request.ProductId, request.ProductName, request.Size, request.Color, request.UnitPrice, request.Quantity));
		}
		else
		{
			basket.Items[basket.Items.IndexOf(item)] = item with { Quantity = item.Quantity + request.Quantity, ProductName = request.ProductName, UnitPrice = request.UnitPrice };
		}

		await store.SaveAsync(basket, cancellationToken);
		logger.LogInformation("Added product {ProductId} to basket for user {UserId}", request.ProductId, userId);
		return basket.ToResponse();
	}
}
