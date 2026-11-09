using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Modules.Common.Application.Messaging;
using Modules.Common.Application.Messaging.Stocks;
using Modules.Stocks.PublicApi;
using Modules.Stocks.PublicApi.Contracts;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NATS.Net;

namespace Modules.Stocks.Features.Messaging;

public sealed class StockInitializationConsumer(
	IOptions<NatsOptions> options,
	IServiceScopeFactory scopeFactory,
	ILogger<StockInitializationConsumer> logger)
	: BackgroundService
{
	private const int MaxDeliveryAttempts = 5;
	private static readonly ActivitySource ActivitySource = new("messaging");

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		var settings = options.Value;
		await using var connection = new NatsClient(settings.Url);
		var jetStream = connection.CreateJetStreamContext();

		var consumerConfig = new ConsumerConfig()
		{
			Name = settings.Entities["Stocks"].Created.DurableConsumer,
			DurableName = settings.Entities["Stocks"].Created.DurableConsumer,
			FilterSubject = settings.Entities["Stocks"].Created.Subject,
			AckPolicy = ConsumerConfigAckPolicy.Explicit,
			AckWait = TimeSpan.FromSeconds(30)
		};

		var consumer = await jetStream.CreateOrUpdateConsumerAsync(
		   settings.Stream,
		   consumerConfig,
		   cancellationToken);

		await foreach (var msg in consumer.ConsumeAsync<StockInitializationRequested>(cancellationToken: cancellationToken))
		{
			try
			{
				await ProcessMessageAsync(msg, cancellationToken);
				await msg.AckAsync(cancellationToken: cancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to process stock initialization message");
			}
		}
	}

	private async Task ProcessMessageAsync(INatsJSMsg<StockInitializationRequested> message, CancellationToken cancellationToken)
	{
		var parentContext = ExtractParentContext(message);

		using var activity = ActivitySource.StartActivity(
			$"consume {nameof(StockInitializationRequested)}", ActivityKind.Consumer, parentContext);

		var deliveryCount = message.Metadata?.NumDelivered ?? 0;
		var seq = message.Metadata?.Sequence.Stream ?? 0;

		using var logScope = logger.BeginScope(new Dictionary<string, object>
		{
			["MessageId"] = message.Data?.MessageId ?? Guid.Empty,
			["ProductName"] = message.Data?.ProductName ?? "(null)",
			["DeliveryCount"] = deliveryCount,
			["StreamSequence"] = seq,
			["Consumer"] = message.Metadata?.Consumer ?? "(unknown)",
		});

		try
		{
			var request = message.Data
				?? throw new InvalidOperationException("The stock initialization message is empty.");

			using var serviceScope = scopeFactory.CreateScope();
			var stockApi = serviceScope.ServiceProvider.GetRequiredService<IStockModuleApi>();

			var result = await stockApi.CreateStockAsync(new CreateStockRequest(request.ProductName,
					request.Quantity,
					request.ProductId,
					request.StockByVariant), cancellationToken)
				.ConfigureAwait(false);

			if (result.IsError)
			{
				if (result.Errors.All(error => error.Code == "Stocks.ProductAlreadyExists"))
				{
					logger.LogInformation("Stock for product {ProductName} was already initialized; acknowledging duplicate message", request.ProductName);
					await message.AckAsync(cancellationToken: cancellationToken);
					return;
				}

				var redeliveryCount = message.Metadata?.NumDelivered ?? 1;
				if (redeliveryCount >= MaxDeliveryAttempts)
				{
					logger.LogError("Giving up on stock initialization for {ProductName} after {Attempts} attempts: {Errors}", request.ProductName, redeliveryCount, result.Errors);
					await message.AckAsync(cancellationToken: cancellationToken);
					return;
				}

				logger.LogError("Could not initialize stock for product {ProductName} (attempt {Attempts}): {Errors}", request.ProductName, redeliveryCount, result.Errors);
				return;
			}

			await message.AckAsync(cancellationToken: cancellationToken);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Error processing stock initialization message");
		}
	}

	private static ActivityContext ExtractParentContext<T>(INatsJSMsg<T> message)
	{
		const string TraceParentHeader = "traceparent";

		if (message.Headers is not null
			&& message.Headers.TryGetValue(TraceParentHeader, out var values)
			&& !StringValues.IsNullOrEmpty(values)
			&& ActivityContext.TryParse(values[0], null, out var context))
		{
			return context;
		}

		return default;
	}
}
