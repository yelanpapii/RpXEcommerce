using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Basket.PublicApi;
using Modules.Catalog.PublicApi;
using Modules.Checkout.Domain.Entities;
using Modules.Checkout.Domain.Enums;
using CheckoutEntity = Modules.Checkout.Domain.Entities.Checkout;
using Modules.Checkout.Features.Features.Shared;
using Modules.Checkout.Features.Payments;
using Modules.Checkout.Infrastructure.Database;
using Modules.Checkout.PublicApi.Contracts;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Shipments.Domain.ValueObjects;
using Modules.Shipments.Features.Features.CreateShipment;
using Modules.Shipments.Features.Features.Shared.Requests;
using Modules.Stocks.PublicApi;
using Modules.Stocks.PublicApi.Contracts;
using Modules.Users.PublicApi;

namespace Modules.Checkout.Features.Features.StartCheckout;

internal interface IStartCheckoutHandler : IHandler
{
	Task<Result<CheckoutResponse>> HandleAsync(string userId, StartCheckoutRequest request, CancellationToken cancellationToken);
}

internal sealed class StartCheckoutHandler(
	IBasketModuleApi basketApi,
	IProductsModuleApi productsApi,
	IUsersModuleApi usersApi,
	IStockModuleApi stockApi,
	ICreateShipmentHandler shipmentHandler,
	IPaymentGateway paymentGateway,
	CheckoutDbContext context,
	ILogger<StartCheckoutHandler> logger) : IStartCheckoutHandler
{
	#pragma warning disable MA0051
	public async Task<Result<CheckoutResponse>> HandleAsync(string userId, StartCheckoutRequest request, CancellationToken cancellationToken)
	{
		var basketResult = await basketApi.GetAsync(userId, cancellationToken);
		if (basketResult.IsError)
		{
			return basketResult.Errors;
		}

		var basket = basketResult.Value!;
		if (basket.Items.Count == 0)
		{
			return CheckoutErrors.EmptyBasket();
		}

		var checkoutExists = await context.Checkouts.AnyAsync(x => x.UserId == userId && x.Status != CheckoutStatus.Failed, cancellationToken);
		if (checkoutExists)
		{
			return CheckoutErrors.AlreadyExists(userId);
		}

		var total = 0m;
		foreach (var item in basket.Items)
		{
			var productResult = await productsApi.GetProductByIdAsync(item.ProductId.ToString(), cancellationToken);
			if (productResult.IsError)
			{
				return CheckoutErrors.ProductNotFound(item.ProductId);
			}

			if (productResult.Value!.Price != item.UnitPrice)
			{
				return CheckoutErrors.PriceChanged(item.ProductId);
			}

			total += productResult.Value.Price * item.Quantity;
		}

		var stockResult = await stockApi.CheckStockAsync(
			new CheckStockRequest(basket.Items.Select(x => new ProductStockDto(x.ProductName, x.Quantity)).ToList()),
			cancellationToken);
		if (stockResult.IsError)
		{
			return stockResult.Errors;
		}

		var profileResult = await usersApi.GetProfileAsync(userId, cancellationToken);
		if (profileResult.IsError)
		{
			return profileResult.Errors;
		}

		var profile = profileResult.Value!;
		if (string.IsNullOrWhiteSpace(profile.Street) || string.IsNullOrWhiteSpace(profile.City) || string.IsNullOrWhiteSpace(profile.Zip))
		{
			return CheckoutErrors.AddressMissing();
		}

		var orderId = Guid.NewGuid().ToString("N");
		var checkout = CheckoutEntity.Create(userId, orderId, total);
		await context.Checkouts.AddAsync(checkout, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);

		var paymentResult = await paymentGateway.AuthorizeAsync(new PaymentRequest(orderId, userId, total), cancellationToken);
		if (paymentResult.IsError)
		{
			checkout.Fail();
			await context.SaveChangesAsync(cancellationToken);
			return paymentResult.Errors;
		}

		checkout.MarkPaid();
		await context.SaveChangesAsync(cancellationToken);

		var shipmentResult = await shipmentHandler.HandleAsync(
			new CreateShipmentRequest(
				orderId,
				new Address { Street = profile.Street, City = profile.City, Zip = profile.Zip },
				request.Carrier,
				profile.Email,
				basket.Items.Select(x => new ShipmentItemRequest(x.ProductName, x.Quantity)).ToList()),
			cancellationToken);
		if (shipmentResult.IsError)
		{
			checkout.Fail();
			await context.SaveChangesAsync(cancellationToken);
			return shipmentResult.Errors;
		}

		checkout.Complete(shipmentResult.Value!.Number);
		await context.SaveChangesAsync(cancellationToken);
		await basketApi.DeleteAsync(userId, cancellationToken);

		logger.LogInformation("Completed checkout {CheckoutId} for user {UserId}", checkout.Id, userId);
		return new CheckoutResponse(checkout.Id, checkout.OrderId, checkout.ShipmentNumber, checkout.Total, checkout.Status, checkout.CreatedAt);
	}
	#pragma warning restore MA0051
}
