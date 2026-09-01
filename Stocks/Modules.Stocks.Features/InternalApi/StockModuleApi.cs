using Modules.Common.Domain.Results;
using Modules.Stocks.Features.Features.CheckStock;
using Modules.Stocks.Features.Features.CreateStock;
using Modules.Stocks.Features.Features.DecreaseStock;
using Modules.Stocks.Features.Features.GetStocksByProductName;
using Modules.Stocks.Features.Features.IncreaseStock;
using Modules.Stocks.PublicApi;
using Modules.Stocks.PublicApi.Contracts;

namespace Modules.Stocks.Features.InternalApi;

internal sealed class StockModuleApi(
    ICheckStockHandler checkStockHandler,
    IDecreaseStockHandler decreaseStockHandler,
    IIncreaseStockHandler increaseStockHandler,
	IGetStocksByProductNameHandler getStocksByProductNameHandler,
	ICreateStockHandler createStockHandler) : IStockModuleApi
{
    public async Task<Result<Success>> CheckStockAsync(
        CheckStockRequest request,
        CancellationToken cancellationToken)
    {
        return await checkStockHandler.HandleAsync(request, cancellationToken);
    }

	public async Task<Result<StockResponse>> GetStockByProductNameAsync(
		GetStocksByProductNameRequest request,
		CancellationToken cancellationToken)
	{
		return await getStocksByProductNameHandler.HandleAsync(request, cancellationToken);
	}

	public async Task<Result<Success>> IncreaseStockAsync(
        IncreaseStockRequest request,
        CancellationToken cancellationToken)
    {
        return await increaseStockHandler.HandleAsync(request, cancellationToken);
    }

    public async Task<Result<Success>> DecreaseStockAsync(
        DecreaseStockRequest request,
        CancellationToken cancellationToken)
    {
        return await decreaseStockHandler.HandleAsync(request, cancellationToken);
    }

	public async Task<Result<CreateStockResponse>> CreateStockAsync(CreateStockRequest request, CancellationToken cancellationToken)
	{
		return await createStockHandler.HandleAsync(request, cancellationToken);
	}
}
