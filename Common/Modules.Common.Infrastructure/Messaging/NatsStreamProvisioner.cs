using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Modules.Common.Application.Messaging;
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
		await jetStream.CreateStreamAsync(
			new StreamConfig(name: settings.Stream, subjects: new[] { $"{settings.SubjectPrefix}.>" }),
			cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
