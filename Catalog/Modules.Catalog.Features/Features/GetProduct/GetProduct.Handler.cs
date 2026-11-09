using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Features.Features.Shared.Converters;
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
	ILogger<GetProductHandler> logger,
	ProductMapper productMapper)
	: IGetProductHandler
{
	public async Task<Result<ProductDto>> HandleAsync(string productId, CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting product {ProductId}", productId);

		var product = await dbContext.Products
			.AsNoTracking()
			.Where(p => p.Id == Guid.Parse(productId))
			.Select(p => new ProductProjection(
				p.Id,
				p.Name,
				p.Description,
				p.CategoryCode.ToString(),
				p.StyleId,
				p.Variants.Select(variant => new ProductVariantProjection(
					variant.Id,
					variant.ProductId,
					variant.Sku,
					variant.Color.Code,
					variant.Size.Code,
					variant.Price,
					variant.Stock)).ToList(),
				p.CreatedAt))
			.FirstOrDefaultAsync(p => p.Id == Guid.Parse(productId), cancellationToken);

		if (product is null)
		{
			logger.LogInformation("Product with ID {ProductId} not found", productId);
			return CatalogErrors.NotFound(productId);
		}

		return productMapper.ToDto(product);
	}
}
