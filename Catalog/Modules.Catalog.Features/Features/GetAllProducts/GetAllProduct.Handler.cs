using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Features.Features.Shared.Converters;
using Modules.Catalog.Infrastructure.Database;
using Modules.Catalog.PublicApi.Contracts;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;

namespace Modules.Catalog.Features.Features.GetAllProducts;

internal interface IGetAllProductsHandler : IHandler
{
	Task<Result<List<ProductDto>>> HandleAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0011:Agregar llaves", Justification = "<pendiente>")]
internal sealed class GetAllProductsHandler(ILogger<GetAllProductsHandler> logger,
	CatalogDbContext catalogDbContext,
	ProductMapper productMapper) : IGetAllProductsHandler
{

	public async Task<Result<List<ProductDto>>> HandleAsync(int page, int pageSize, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("Getting products page {Page} size {PageSize}", page, pageSize);

		if (page < 1) page = 1;
		if (pageSize < 1) pageSize = 10;

		var products = await catalogDbContext.Products
			.AsNoTracking()
			.OrderBy(p => p.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.Select(product => new ProductProjection(
				product.Id,
				product.Name,
				product.Description,
				product.CategoryCode.ToString() ?? string.Empty,
				product.StyleId,
				product.Variants.Select(variant => new ProductVariantProjection(
					variant.Id,
					variant.ProductId,
					variant.Sku,
					variant.Color.Code,
					variant.Size.Code,
					variant.Price,
					variant.Stock)).ToList(),
				product.CreatedAt))
			.ToListAsync(cancellationToken);

		return products.Select(productMapper.ToDto).ToList();
	}
}
