using Microsoft.Extensions.Logging;
using Modules.Basket.Domain.Repositories;
using Modules.Basket.Infrastructure.Repositories;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;

namespace Modules.Basket.Features.Features.DeleteBasket;

internal interface IDeleteBasketHandler : IHandler
{
	Task<Result<Success>> HandleAsync(string userId, CancellationToken cancellationToken);
}

internal sealed class DeleteBasketHandler(IBasketStore store, ILogger<DeleteBasketHandler> logger) : IDeleteBasketHandler
{
	public async Task<Result<Success>> HandleAsync(string userId, CancellationToken cancellationToken)
	{
		await store.DeleteAsync(userId, cancellationToken);
		logger.LogInformation("Deleted basket for user {UserId}", userId);
		return Result.Success;
	}
}
