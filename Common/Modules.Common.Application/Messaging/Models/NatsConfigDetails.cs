using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Common.Application.Messaging.Models;

public class NatsConfigDetails
{
	public string Subject { get; set; } = string.Empty;
	public string DurableConsumer { get; set; } = string.Empty;
}
