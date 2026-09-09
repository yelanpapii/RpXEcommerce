namespace Modules.Common.Application.Messaging;

public sealed class RabbitMqOptions
{
	public required string HostName { get; init; }
	public required int Port { get; init; }
	public required string UserName { get; init; } 
	public required string Password { get; init; }
	public required string Exchange { get; init; }
	public required Dictionary<string, string> Queues { get; init; }
}
