using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modules.Common.Application.Messaging;
using Modules.Common.Application.Messaging.Stocks;
using NATS.Client;
using NATS.Client.Core;
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
	private static readonly ActivitySource ActivitySource = new("messaging");

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



	public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
	{
		using var activity = ActivitySource.StartActivity($"publish {typeof(TMessage).Name}", ActivityKind.Producer);
		var subject = GetSubject<TMessage>();

		var headers = new NatsHeaders();
		if (activity is not null)
		{
			// The carrier is always the NatsHeaders instance passed below; the pattern match
			// keeps nullability analyzers happy without weakening runtime behavior.
			DistributedContextPropagator.Current.Inject(activity, headers,
				(carrier, key, value) =>
				{
					if (carrier is NatsHeaders natsHeaders)
					{
						natsHeaders[key] = value;
					}
				});
		}

		var ack = await _jetStream.PublishAsync(subject, message, headers: headers, cancellationToken: cancellationToken);
		ack.EnsureSuccess();

		if (activity is not null)
		{
			activity.SetTag("messaging.system", "nats");
			activity.SetTag("messaging.destination", subject);
			activity.SetTag("messaging.nats.stream", ack.Stream);
			activity.SetTag("messaging.nats.sequence", ack.Seq);
		}

		_logger.LogInformation("Published {MessageType} on {Subject} (stream {Stream}, seq {Seq})",
			typeof(TMessage).Name, subject, ack.Stream, ack.Seq);
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
