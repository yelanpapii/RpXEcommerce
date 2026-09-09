using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modules.Common.Application.Messaging;
using Modules.Stocks.PublicApi;
using Modules.Stocks.PublicApi.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Modules.Stocks.Features.Messaging;

public sealed class StockInitializationConsumer(
	IOptions<RabbitMqOptions> options,
	IServiceScopeFactory scopeFactory,
	ILogger<StockInitializationConsumer> logger,
	IConnectionFactory connectionFactory)
	: BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		var settings = options.Value;
		await using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
		await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

		var consumer = new AsyncEventingBasicConsumer(channel);

		ExecuteMessage(consumer, channel, cancellationToken);

		await channel.ExchangeDeclareAsync(
			settings.Exchange,
			ExchangeType.Direct,
			durable: true,
			autoDelete: false,
			arguments: null,
			cancellationToken: cancellationToken);

		await channel.QueueDeclareAsync(
			settings.Queues[nameof(StockInitializationRequested)],
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null,
			cancellationToken: cancellationToken);

		await channel.QueueBindAsync(
			settings.Queues[nameof(StockInitializationRequested)],
			settings.Exchange,
			nameof(StockInitializationRequested),
			arguments: null,
			cancellationToken: cancellationToken);

		await channel.BasicConsumeAsync(
			settings.Queues[nameof(StockInitializationRequested)],
			autoAck: false,
			consumer: consumer,
			cancellationToken: cancellationToken);

		await Task.Delay(Timeout.Infinite, cancellationToken);
	}

	private void ExecuteMessage(AsyncEventingBasicConsumer consumer, IChannel channel, CancellationToken cancellationToken)
	{
		consumer.ReceivedAsync += async (_, eventArgs) =>
		{
			try
			{
				var message = JsonSerializer.Deserialize<StockInitializationRequested>(eventArgs.Body.Span)
					?? throw new InvalidOperationException("The stock initialization message is empty.");

				using var scope = scopeFactory.CreateScope();

				var stockApi = scope.ServiceProvider.GetRequiredService<IStockModuleApi>();

				var result = await stockApi.CreateStockAsync(
					new CreateStockRequest(message.ProductName, message.Quantity),
					cancellationToken);

				if (result.IsError)
				{
					if (result.Errors.All(error => error.Code == "Stocks.ProductAlreadyExists"))
					{
						logger.LogInformation("Stock for product {ProductName} was already initialized; acknowledging duplicate message", message.ProductName);
						await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
						return;
					}

					logger.LogError("Could not initialize stock for product {ProductName}: {Errors}", message.ProductName, result.Errors);
					await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
					return;
				}

				await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Error processing stock initialization message");
				await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
			}
		};

	}
}
