using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Stocks.Features.Features.Shared.Errors;
using Modules.Stocks.Infrastructure.Database;
using Modules.Stocks.PublicApi.Contracts;

namespace Modules.Stocks.Features.Features.DecreaseStock;

internal interface IDecreaseStockHandler : IHandler
{
    Task<Result<Success>> HandleAsync(DecreaseStockRequest request, CancellationToken cancellationToken);
}

internal sealed class DecreaseStockHandler(
    StocksDbContext dbContext,
    IValidator<DecreaseStockRequest> validator)
    : IDecreaseStockHandler
{
    public async Task<Result<Success>> HandleAsync(
        DecreaseStockRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToDomainErrors();
        }

        var requestedProducts = request.Products
            .GroupBy(product => product.ProductName)
            .ToDictionary(group => group.Key, group => group.Sum(product => product.Quantity));

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var productNames = requestedProducts.Keys.ToList();
        var existingProductNames = await dbContext.ProductStocks
            .Where(stock => productNames.Contains(stock.ProductName))
            .Select(stock => stock.ProductName)
            .ToListAsync(cancellationToken);

        var missingProductName = productNames.FirstOrDefault(name => !existingProductNames.Contains(name));
        if (missingProductName is not null)
        {
            await transaction.RollbackAsync(cancellationToken);
            return StockErrors.ProductNotFound(missingProductName);
        }

        foreach (var (productName, quantity) in requestedProducts)
        {
            var updatedRows = await dbContext.ProductStocks
                .Where(stock => stock.ProductName == productName && stock.AvailableQuantity >= quantity)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(stock => stock.AvailableQuantity, stock => stock.AvailableQuantity - quantity)
                    .SetProperty(stock => stock.LastUpdatedAt, _ => DateTime.UtcNow), cancellationToken);

            if (updatedRows == 0)
            {
                var availableQuantity = await dbContext.ProductStocks
                    .Where(stock => stock.ProductName == productName)
                    .Select(stock => stock.AvailableQuantity)
                    .SingleAsync(cancellationToken);

                await transaction.RollbackAsync(cancellationToken);
                return StockErrors.InsufficientStocks(productName, quantity, availableQuantity);
            }
        }

        await transaction.CommitAsync(cancellationToken);
        return Result.Success;
    }
}
