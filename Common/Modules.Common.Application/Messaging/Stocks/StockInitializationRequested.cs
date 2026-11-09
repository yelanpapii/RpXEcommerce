namespace Modules.Common.Application.Messaging.Stocks;

public sealed record StockInitializationRequested(
	Guid MessageId,
	Guid ProductId,
	string ProductName,
	IReadOnlyDictionary<string, string> StockByVariant,
	int Quantity);
