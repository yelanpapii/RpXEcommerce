using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Stocks.Features.Features.Shared.Errors;
using Modules.Stocks.Infrastructure.Database;
using Modules.Stocks.PublicApi.Contracts;

namespace Modules.Stocks.Features.Features.GetStocksByProductName;

internal interface IGetStocksByProductIdHandler : IHandler
{
    Task<Result<List<StockResponse>>> HandleAsync(GetStocksByProductIdRequest request, CancellationToken cancellationToken);
}

internal sealed class GetStocksByProductIdHandler(
    StocksDbContext context,
    ILogger<GetStocksByProductIdHandler> logger)
    : IGetStocksByProductIdHandler
{
	public async Task<Result<List<StockResponse>>> HandleAsync(
	GetStocksByProductIdRequest request,
	CancellationToken cancellationToken)
	{
		var query = context.ProductStocks
			.AsNoTracking()
			.Where(x => x.ProductId == request.ProductId);

		if (request.VariantId is { } variantId)
		{
			query = query.Where(x => x.VariantId == variantId);
		}

		var stocks = await query
			.Select(x => new StockResponse(
				x.ProductId.ToString(),
				x.VariantId.ToString(),
				x.ProductName,
				x.AvailableQuantity))
			.ToListAsync(cancellationToken);

		if (stocks.Count == 0)
		{
			logger.LogInformation("Product '{ProductId}' not found in stock", request.ProductId);
			return StockErrors.ProductNotFound(request.ProductId.ToString());
		}

		logger.LogInformation("Retrieved {Count} stock row(s) for product '{ProductId}'",
			stocks.Count, request.ProductId);

		return stocks;
	}
}
