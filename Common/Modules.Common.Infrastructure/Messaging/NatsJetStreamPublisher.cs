using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modules.Common.Application.Messaging;
using NATS.Client;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NATS.Net;

namespace Modules.Common.Infrastructure.Messaging;

public sealed class NatsJetStreamPublisher : INatsPublisher, IAsyncDisposable
{
	private readonly SemaphoreSlim _initLock = new(1, 1);
	private readonly NatsOptions _options;
	private readonly NatsClient _client;
	private readonly ILogger<NatsJetStreamPublisher> _logger;
	private readonly INatsJSContext _jetStream;

	public NatsJetStreamPublisher(
		NatsClient client,
		IOptions<NatsOptions> options,
		ILogger<NatsJetStreamPublisher> logger)
	{
		_client = client;
		_options = options.Value;
		_logger = logger;
		_jetStream = client.CreateJetStreamContext();
	}

	public async Task PublishAsync<TMessage>(
		TMessage message,
		CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var subject = GetSubject<TMessage>();
		var ack = await _jetStream.PublishAsync(subject, message, cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		ack.EnsureSuccess();

		_logger.LogInformation("Published NATS JetStream message {MessageType} on {Subject}", typeof(TMessage).Name, subject);
	}


	private string GetSubject<TMessage>() =>
	$"{_options.SubjectPrefix}.{ToSubjectToken(typeof(TMessage).Name)}";

	private static string ToSubjectToken(string typeName) =>
		string.Concat(typeName.Select((c, i) =>
			i > 0 && char.IsUpper(c) ? $"-{char.ToLowerInvariant(c)}" : char.ToLowerInvariant(c).ToString()));

	public async ValueTask DisposeAsync()
	{
		_initLock.Dispose();
		await _client.DisposeAsync().ConfigureAwait(false);
	}
}
