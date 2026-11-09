using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Infrastructure.Database;
using Modules.Common.Application.Messaging.Stocks;
using Modules.Common.Infrastructure.Messaging;

namespace Modules.Catalog.Infrastructure.Messaging;

/// <summary>
/// Background service that processes the catalog outbox:
/// queries pending messages, claims (locks) records to avoid duplicates,
/// publishes events (currently only `StockInitializationRequested`) via NATS,
/// and updates the messages' state in the database (processed, attempts, errors).
/// </summary>
public sealed class CatalogOutboxDispatcher(
	IServiceScopeFactory scopeFactory,
	INatsPublisher publisher,
	ILogger<CatalogOutboxDispatcher> logger)
	: BackgroundService
{
	private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
	private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(1);

	/// <summary>
	/// Main service loop that periodically attempts to process pending messages
	/// until cancellation is requested.
	/// </summary>
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

	/// <summary>
	/// Queries up to 20 unprocessed and unlocked messages, claims each by setting a lock,
	/// publishes the message and updates its state in the database. On failure increments
	/// the attempt count and records the error.
	/// </summary>
	private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
	{
		using var scope = scopeFactory.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
		var now = DateTimeOffset.UtcNow;

		var pendingCatalogMessages = await dbContext.OutboxMessages
			.Where(message => message.ProcessedOnUtc == null
				&& (message.LockedUntilUtc == null || message.LockedUntilUtc < now))
			.OrderBy(message => message.OccurredOnUtc)
			.Take(20)
			.ToListAsync(cancellationToken);

		foreach (var message in pendingCatalogMessages)
		{
			var claimedMessages = await dbContext.OutboxMessages
				.Where(candidate => candidate.Id == message.Id
					&& candidate.ProcessedOnUtc == null
					&& (candidate.LockedUntilUtc == null || candidate.LockedUntilUtc < now))
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(candidate => candidate.LockedUntilUtc, now.Add(LockDuration)), cancellationToken);

			if (claimedMessages == 0)
			{
				continue;
			}

			using (logger.BeginScope(new Dictionary<string, object>
			{
				["MessageId"] = message.Id,
				["MessageType"] = message.Type,
				["OutboxAttempt"] = message.AttemptCount
			}))
			{
				try
				{
					await PublishAsync(message, cancellationToken);

					logger.LogInformation("Outbox message published and already proccesed.");

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
	}

	/// <summary>
	/// Deserializes the payload and publishes the event using the <see cref="INatsPublisher"/>.
	/// Throws an exception if the message type is not supported or the payload is empty.
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	private Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken)
	{
		if (message.Type != nameof(StockInitializationRequested))
		{
			throw new InvalidOperationException($"Unsupported outbox message type '{message.Type}'.");
		}

		var payload = JsonSerializer.Deserialize<StockInitializationRequested>(message.Payload);

		return publisher.PublishAsync(payload, cancellationToken);
	}
}
