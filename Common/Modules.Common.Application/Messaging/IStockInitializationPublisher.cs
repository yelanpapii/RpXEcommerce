namespace Modules.Common.Application.Messaging;

public interface INatsPublisher
{
	Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken);
}
