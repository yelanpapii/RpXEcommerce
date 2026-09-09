using System.Text.Json;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modules.Common.Application.Messaging;
using RabbitMQ.Client;
using IConnectionFactory = RabbitMQ.Client.IConnectionFactory;

namespace Modules.Common.Infrastructure.Messaging;

public sealed class RabbitMqPublisher(
	IOptions<RabbitMqOptions> options,
	ILogger<RabbitMqPublisher> logger,
	IConnectionFactory connectionFactory)
	: IRabbitMqPublisher
{
	public async Task PublishAsync<TMessage>(
		TMessage message,
		CancellationToken cancellationToken)
	{
		var settings = options.Value;

		await using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken: cancellationToken);
		await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

		await channel.ExchangeDeclareAsync(
			settings.Exchange,
			ExchangeType.Direct,
			durable: true,
			autoDelete: false,
			arguments: null,
			cancellationToken: cancellationToken);

		var body = JsonSerializer.SerializeToUtf8Bytes(message);
		var properties = new BasicProperties
		{
			Persistent = true,
			ContentType = "application/json",
			Type = typeof(TMessage).Name
		};

		await channel.BasicPublishAsync(
			settings.Exchange,
			typeof(TMessage).Name,
			mandatory: false,
			basicProperties: properties,
			body,
			cancellationToken);

		logger.LogInformation("Published RabbitMQ message {MessageType}", typeof(TMessage).Name);
	}
}
