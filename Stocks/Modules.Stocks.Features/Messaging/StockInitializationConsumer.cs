using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modules.Common.Application.Messaging;
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

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		var settings = options.Value;
		await using var connection = new NatsClient(settings.Url);
		var jetStream = connection.CreateJetStreamContext();

		var consumerConfig = new ConsumerConfig()
		{
			Name = settings.StockInitializationDurableConsumer,
			DurableName = settings.StockInitializationDurableConsumer,
			FilterSubject = settings.StockInitializationSubject,
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
		try
		{
			var request = message.Data
				?? throw new InvalidOperationException("The stock initialization message is empty.");

			using var scope = scopeFactory.CreateScope();
			var stockApi = scope.ServiceProvider.GetRequiredService<IStockModuleApi>();
			var result = await stockApi.CreateStockAsync(
				new CreateStockRequest(request.ProductName, request.Quantity),
				cancellationToken).ConfigureAwait(false);

			if (result.IsError)
			{
				if (result.Errors.All(error => error.Code == "Stocks.ProductAlreadyExists"))
				{
					logger.LogInformation("Stock for product {ProductName} was already initialized; acknowledging duplicate message", request.ProductName);
					await message.AckAsync(cancellationToken: cancellationToken);
					return;
				}

				var deliveryCount = message.Metadata?.NumDelivered ?? 1;
				if (deliveryCount >= MaxDeliveryAttempts)
				{
					logger.LogError("Giving up on stock initialization for {ProductName} after {Attempts} attempts: {Errors}", request.ProductName, deliveryCount, result.Errors);
					await message.AckAsync(cancellationToken: cancellationToken);
					return;
				}

				logger.LogError("Could not initialize stock for product {ProductName} (attempt {Attempts}): {Errors}", request.ProductName, deliveryCount, result.Errors);
				return; // no ack -> redeliver
			}

			await message.AckAsync(cancellationToken: cancellationToken);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Error processing stock initialization message");
			// Without an ACK JetStream will redeliver the message after AckWait.
		}
	}
}
