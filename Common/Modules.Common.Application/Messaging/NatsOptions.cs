using Modules.Common.Application.Messaging.Models;

namespace Modules.Common.Application.Messaging;

public sealed class NatsOptions
{
	public required string Url { get; set; }
	public required string Stream { get; set; }
	public required string SubjectPrefix { get; set; }
	public Dictionary<string, EntityEvents> Entities { get; set; } = new();
}
