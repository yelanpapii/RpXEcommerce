using System;
using System.Collections.Generic;
using System.Text;
using Modules.Catalog.Domain.Entities;
using Modules.Catalog.Features.Tracing;
using Modules.Catalog.PublicApi;
using Modules.Catalog.PublicApi.Contracts;
using Modules.Common.Domain.Results;

namespace Modules.Catalog.Features.InternalApi.Decorators;

public class TracedProductsModuleApi(IProductsModuleApi innerApi) : IProductsModuleApi
{
	public async Task<Result<Success>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
	{
		using var activity = CatalogActivitySource.Instance.StartActivity($"{CatalogActivitySource.Instance.Name}.create.product");

		activity?.AddTag("product.name", request.Name);
		activity?.AddTag("product.description", request.Description);
		activity?.AddTag("product.variant_count", request.ProductVariant?.Count ?? 0);
		activity?.AddTag("product.initial_stock", request.ProductVariant?.Sum(variant => variant.Stock) ?? 0);

		try
		{
			var response = await innerApi.CreateProductAsync(request, cancellationToken);

			activity?.SetStatus(response.IsSuccess ? System.Diagnostics.ActivityStatusCode.Ok : System.Diagnostics.ActivityStatusCode.Error);

			return response;
		}
		catch (Exception ex)
		{
			activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
			activity?.SetTag("error.message", ex.Message);
			throw;
		}
	}

	public async Task<Result<ProductDto>> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default)
	{
		using var activity = CatalogActivitySource.Instance.StartActivity($"{CatalogActivitySource.Instance.Name}.get.product.by.id");

		activity?.AddTag("product.id", productId);

		try
		{
			var response = await innerApi.GetProductByIdAsync(productId, cancellationToken);

			activity?.SetStatus(response.IsSuccess ? System.Diagnostics.ActivityStatusCode.Ok : System.Diagnostics.ActivityStatusCode.Error);

			return response;
		}
		catch (Exception ex)
		{
			activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
			activity?.SetTag("error.message", ex.Message);
			throw;
		}
	}

	public async Task<Result<List<ProductDto>>> GetProductsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
	{
		using var activity = CatalogActivitySource.Instance.StartActivity($"{CatalogActivitySource.Instance.Name}.get.products");

		activity?.AddTag("page", page.ToString());
		activity?.AddTag("pageSize", pageSize.ToString());

		try
		{
			var response = await innerApi.GetProductsAsync(page, pageSize, cancellationToken);

			activity?.SetStatus(response.IsSuccess ? System.Diagnostics.ActivityStatusCode.Ok : System.Diagnostics.ActivityStatusCode.Error);

			return response;
		}
		catch (Exception ex)
		{
			activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
			activity?.SetTag("error.message", ex.Message);
			throw;
		}
	}
}
