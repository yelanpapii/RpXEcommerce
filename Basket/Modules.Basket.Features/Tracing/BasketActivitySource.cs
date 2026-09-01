using System.Diagnostics;

namespace Modules.Basket.Features.Tracing;

internal static class BasketActivitySource
{
	internal static readonly ActivitySource Instance = new("basket");
}
