using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Modules.Common.Application.Messaging;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using NATS.Net;

namespace Modules.Common.Infrastructure.Messaging;

public sealed class NatsStreamProvisioner(
	IOptions<NatsOptions> options,
	NatsClient client) : IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var settings = options.Value;
		var jetStream = client.CreateJetStreamContext();
		var streamConfig = new StreamConfig(name: settings.Stream, subjects: [$"{settings.SubjectPrefix}.>"])
		{
			Storage = StreamConfigStorage.File,
			MaxAge = TimeSpan.FromDays(7),
			Retention = StreamConfigRetention.Limits,
			MaxBytes = 10L * 1024 * 1024 * 1024,
			NumReplicas = 1,
			Discard = StreamConfigDiscard.Old,
		};

		await jetStream.CreateOrUpdateStreamAsync(streamConfig, cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
