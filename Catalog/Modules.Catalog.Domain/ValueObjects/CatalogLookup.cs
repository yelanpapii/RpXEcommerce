using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Catalog.Domain.ValueObjects;

/// <summary>
/// Entry of a catalog (color, size, etc). The code is the unique identifier, the name is the display name, and the sort order is used to order the entries in a list.
/// </summary>
public sealed record CatalogEntry(string Code, string Name, int SortOrder = 0);
