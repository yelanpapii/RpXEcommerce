using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Common.Application.Messaging.Models;

public class EntityEvents
{
	public NatsConfigDetails Created { get; set; } = new();
	public NatsConfigDetails Updated { get; set; } = new();
	public NatsConfigDetails Deleted { get; set; } = new();
}
