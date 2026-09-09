namespace Modules.Common.Application.Messaging;

public interface IRabbitMqPublisher
{
	Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken);
}
