namespace Modules.Common.Application.Messaging;

public sealed class NatsOptions
{
	public required string Url { get; set; }
	public string Stream { get; set; } = "modular-monolith.events";
	public string SubjectPrefix { get; set; } = "modular-monolith.events";
	public string StockInitializationSubject { get; set; } = "stocks.initialization.requested";
	public string StockInitializationDurableConsumer { get; set; } = "stocks-initialization";
}
