using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Domain.Entities;
using Modules.Catalog.Features.Features.CreateProduct;
using Modules.Catalog.Features.Features.GetAllProducts;
using Modules.Catalog.Features.Features.GetProduct;
using Modules.Catalog.Infrastructure.Database;
using Modules.Catalog.PublicApi;
using Modules.Catalog.PublicApi.Contracts;
using Modules.Common.Domain.Results;

namespace Modules.Catalog.Features.InternalApi;

internal sealed class ProductsModuleApi(ICreateProductHandler createProductHandler,
	IGetProductHandler getProductHandler,
	IGetAllProductsHandler getAllProductsHandler) : IProductsModuleApi
{
	public async Task<Result<List<ProductDto>>> GetProductsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
	{
		return await getAllProductsHandler.HandleAsync(page, pageSize, cancellationToken);
	}

	public async Task<Result<ProductDto>> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default)
	{
		return await getProductHandler.HandleAsync(productId, cancellationToken);
	}

	public async Task<Result<Success>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
	{
		return await createProductHandler.HandleAsync(request, cancellationToken);
	}
}
