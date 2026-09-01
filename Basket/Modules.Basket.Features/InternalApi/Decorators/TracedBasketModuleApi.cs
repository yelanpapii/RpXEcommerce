using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Modules.Basket.Features.Tracing;
using Modules.Basket.PublicApi;
using Modules.Basket.PublicApi.Contracts;
using Modules.Common.Domain.Results;

namespace Modules.Basket.Features.InternalApi.Decorators;

public class TracedBasketModuleApi(IBasketModuleApi inner) : IBasketModuleApi
{
	public async Task<Result<BasketResponse>> AddItemAsync(string userId, AddBasketItemRequest request, CancellationToken cancellationToken = default)
	{
		using var activity = BasketActivitySource.Instance.StartActivity($"{BasketActivitySource.Instance.Name}.add-item");

		activity?.SetTag("module", BasketActivitySource.Instance.Name);
		activity?.SetTag("operation", "AddItem");
		activity?.SetTag("product.id", request.ProductId);
		activity?.SetTag("product.name", request.ProductName);
		activity?.SetTag("product.size", request.Size);

		try
		{
			var response = await inner.AddItemAsync(userId, request, cancellationToken);

			activity?.SetStatus(response.IsSuccess ? ActivityStatusCode.Ok : ActivityStatusCode.Error);

			return response;
		}
		catch (Exception ex)
		{
			activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
			activity?.SetTag("error.message", ex.Message);
			throw;
		}
	}

	public async Task<Result<Success>> DeleteAsync(string userId, CancellationToken cancellationToken = default)
	{
		using var activity = BasketActivitySource.Instance.StartActivity($"{BasketActivitySource.Instance.Name}.delete-item");

		activity?.SetTag("module", BasketActivitySource.Instance.Name);
		activity?.SetTag("operation", "DeleteItem");
		activity?.SetTag("user.id", userId);

		try
		{
			var response = await inner.DeleteAsync(userId, cancellationToken);

			activity?.SetStatus(response.IsSuccess ? ActivityStatusCode.Ok : ActivityStatusCode.Error);

			return response;
		}
		catch (Exception ex)
		{
			activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
			activity?.SetTag("error.message", ex.Message);
			throw;
		}
	}

	public async Task<Result<BasketResponse>> UpdateItemAsync(string userId, UpdateBasketItemRequest request, CancellationToken cancellationToken = default)
	{
		using var activity = BasketActivitySource.Instance.StartActivity($"{BasketActivitySource.Instance.Name}.update-item");

		activity?.SetTag("module", BasketActivitySource.Instance.Name);
		activity?.SetTag("operation", "UpdateItem");
		activity?.SetTag("user.id", userId);
		activity?.SetTag("product.id", request.ProductId);
		activity?.SetTag("product.size", request.Size);

		try
		{
			var response = await inner.UpdateItemAsync(userId, request, cancellationToken);

			activity?.SetStatus(response.IsSuccess ? ActivityStatusCode.Ok : ActivityStatusCode.Error);

			return response;
		}
		catch (Exception ex)
		{
			activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
			activity?.SetTag("error.message", ex.Message);
			throw;
		}
	}
}
