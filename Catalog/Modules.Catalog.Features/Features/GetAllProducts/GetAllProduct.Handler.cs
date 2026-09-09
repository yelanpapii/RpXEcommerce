using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
	CatalogDbContext catalogDbContext) : IGetAllProductsHandler
{
	
	public async Task<Result<List<ProductDto>>> HandleAsync(int page, int pageSize, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("Getting products page {Page} size {PageSize}", page, pageSize);

		if (page < 1) page = 1;
		if (pageSize < 1) pageSize = 10;

		var products = await catalogDbContext.Products
			.AsNoTracking()
			.Include(p => p.Variants)
			.OrderBy(p => p.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return products.Select(product => new ProductDto(
			product.Id,
			product.Name,
			product.Description,
			product.CategoryCode,
			product.StyleId,
			product.Variants.Select(variant => new ProductVariantDto(
				variant.Id,
				variant.Sku.ToString(),
				variant.ColorName,
				variant.ColorCode,
				variant.SizeName,
				variant.Price,
				variant.Stock)).ToList(),
			product.CreatedAt)).ToList();
	}
}
