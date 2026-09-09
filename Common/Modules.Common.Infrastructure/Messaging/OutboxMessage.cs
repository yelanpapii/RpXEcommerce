namespace Modules.Common.Infrastructure.Messaging;

public sealed class OutboxMessage
{
	public Guid Id { get; set; }
	public string Type { get; set; } = null!;
	public string Payload { get; set; } = null!;
	public DateTimeOffset OccurredOnUtc { get; set; }
	public DateTimeOffset? ProcessedOnUtc { get; set; }
	public DateTimeOffset? LockedUntilUtc { get; set; }
	public int AttemptCount { get; set; }
	public string? LastError { get; set; }
}
