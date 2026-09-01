using System.Diagnostics;

namespace Modules.Catalog.Features.Tracing;

internal static class CatalogActivitySource
{
	internal static readonly ActivitySource Instance = new("catalog");
}
