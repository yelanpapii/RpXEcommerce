using Modules.Catalog.Domain.Entities;
using Modules.Catalog.Domain.ValueObjects;

namespace Modules.Catalog.Domain.Lookups.Interfaces;

/// <summary>
/// Resolves and validates size codes against the configured catalog.
/// </summary>
public interface ISizeCatalog
{
	/// <summary>
	/// Write/validation-path lookup. Returns the size value object when the code exists in the
	/// catalog, or <see langword="null"/> when the code is missing, blank or unknown.
	/// This method never throws: callers decide whether an absent code is an error.
	/// </summary>
	/// <param name="code">The size code to resolve (matched case-insensitively).</param>
	/// <returns>The matching <see cref="Size"/>, or <see langword="null"/> when not found.</returns>
	Size? Resolve(string code);

	string GetDisplayName(Size size);

	/// <summary>
	/// Read-path lookup. Gets the display name directly from a stored size code, without
	/// reconstructing the <see cref="Size"/> value object. Used when projecting DTOs straight from
	/// query results (avoids per-row value-object validation during in-memory enrichment).
	/// This method never throws and never validates the code: when the code is absent from the
	/// catalog, the raw stored code is returned as a graceful fallback.
	/// </summary>
	/// <param name="code">The stored size code to look up.</param>
	/// <returns>The configured display name, or the raw <paramref name="code"/> when not found.</returns>
	string GetDisplayName(string code);

	/// <summary>
	/// Read-path enumeration of all configured sizes. Never throws.
	/// </summary>
	IReadOnlyCollection<CatalogEntry> GetAll();
}
