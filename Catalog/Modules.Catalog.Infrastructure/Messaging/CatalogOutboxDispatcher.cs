using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Infrastructure.Database;
using Modules.Common.Application.Messaging;
using Modules.Common.Infrastructure.Messaging;

namespace Modules.Catalog.Infrastructure.Messaging;

public sealed class CatalogOutboxDispatcher(
	IServiceScopeFactory scopeFactory,
	IRabbitMqPublisher publisher,
	ILogger<CatalogOutboxDispatcher> logger)
	: BackgroundService
{
	private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
	private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(1);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await ProcessPendingMessagesAsync(stoppingToken);
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Error processing the Catalog outbox");
			}

			await Task.Delay(PollingInterval, stoppingToken);
		}
	}

	private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
	{
		using var scope = scopeFactory.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
		var now = DateTimeOffset.UtcNow;

		var pendingMessages = await dbContext.OutboxMessages
			.Where(message => message.ProcessedOnUtc == null
				&& (message.LockedUntilUtc == null || message.LockedUntilUtc < now))
			.OrderBy(message => message.OccurredOnUtc)
			.Take(20)
			.ToListAsync(cancellationToken);

		foreach (var message in pendingMessages)
		{
			var claimed = await dbContext.OutboxMessages
				.Where(candidate => candidate.Id == message.Id
					&& candidate.ProcessedOnUtc == null
					&& (candidate.LockedUntilUtc == null || candidate.LockedUntilUtc < now))
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(candidate => candidate.LockedUntilUtc, now.Add(LockDuration)), cancellationToken);

			if (claimed == 0)
			{
				continue;
			}

			try
			{
				await PublishAsync(message, cancellationToken);

				await dbContext.OutboxMessages
					.Where(candidate => candidate.Id == message.Id)
					.ExecuteUpdateAsync(setters => setters
						.SetProperty(candidate => candidate.ProcessedOnUtc, DateTimeOffset.UtcNow)
						.SetProperty(candidate => candidate.LockedUntilUtc, (DateTimeOffset?)null), cancellationToken);
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Error publishing outbox message {MessageId}", message.Id);

				await dbContext.OutboxMessages
					.Where(candidate => candidate.Id == message.Id)
					.ExecuteUpdateAsync(setters => setters
						.SetProperty(candidate => candidate.AttemptCount, candidate => candidate.AttemptCount + 1)
						.SetProperty(candidate => candidate.LastError, exception.Message)
						.SetProperty(candidate => candidate.LockedUntilUtc, (DateTimeOffset?)null), cancellationToken);
			}
		}
	}

	private Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken)
	{
		if (message.Type != nameof(StockInitializationRequested))
		{
			throw new InvalidOperationException($"Unsupported outbox message type '{message.Type}'.");
		}

		var payload = JsonSerializer.Deserialize<StockInitializationRequested>(message.Payload)
			?? throw new InvalidOperationException($"Outbox message '{message.Id}' has an empty payload.");

		return publisher.PublishAsync(payload, cancellationToken);
	}
}
