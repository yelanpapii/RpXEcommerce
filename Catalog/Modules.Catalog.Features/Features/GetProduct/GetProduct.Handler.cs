using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Infrastructure.Database;
using Modules.Catalog.PublicApi.Contracts;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Catalog.Features.Features.Shared.Errors;

namespace Modules.Catalog.Features.Features.GetProduct;

internal interface IGetProductHandler : IHandler
{
	Task<Result<ProductDto>> HandleAsync(string productId, CancellationToken cancellationToken);
}

internal sealed class GetProductHandler(
	CatalogDbContext dbContext,
	ILogger<GetProductHandler> logger)
	: IGetProductHandler
{
	public async Task<Result<ProductDto>> HandleAsync(string productId, CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting product {ProductId}", productId);

		var product = await dbContext.Products
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Id == Guid.Parse(productId), cancellationToken);

		if (product is null)
		{
			logger.LogInformation("Product with ID {ProductId} not found", productId);
			return CatalogErrors.NotFound(productId);
		}

		return new ProductDto(product.Id, product.Name, product.Description, product.SKU, product.Price, product.CreatedAt);
	}
}
