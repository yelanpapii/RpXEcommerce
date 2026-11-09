namespace Modules.Common.Application.Messaging.Stocks;

public interface INatsPublisher
{
	Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken);
}
