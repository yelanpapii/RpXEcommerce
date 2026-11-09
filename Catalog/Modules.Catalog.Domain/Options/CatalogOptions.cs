using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Catalog.Domain.Options;

public sealed class ProductCatalogOptions
{
	public const string SectionName = "ProductCatalog";

	public List<CatalogEntryOptions> Colors { get; init; } = new();
	public List<CatalogEntryOptions> Sizes { get; init; } = new();
}

public sealed class CatalogEntryOptions
{
	public string Code { get; init; } = null!;
	public string Name { get; init; } = null!;
	public int SortOrder { get; init; }
}

