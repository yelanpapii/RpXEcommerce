namespace Modules.Common.Application.Messaging;

public sealed record StockInitializationRequested(
	Guid MessageId,
	string ProductName,
	int Quantity);
