using Modules.Common.Domain.Results;
using Modules.Stocks.PublicApi.Contracts;

namespace Modules.Stocks.PublicApi;

public interface IStockModuleApi
{
    Task<Result<Success>> CheckStockAsync(
        CheckStockRequest request,
        CancellationToken cancellationToken);

	Task<Result<CreateStockResponse>> CreateStockAsync(
		CreateStockRequest request,
		CancellationToken cancellationToken);

	Task<Result<Success>> IncreaseStockAsync(
		IncreaseStockRequest request,
		CancellationToken cancellationToken);

	Task<Result<Success>> DecreaseStockAsync(
        DecreaseStockRequest request,
        CancellationToken cancellationToken);

	Task<Result<StockResponse>> GetStockByProductNameAsync(
		GetStocksByProductNameRequest request,
		CancellationToken cancellationToken);
}
