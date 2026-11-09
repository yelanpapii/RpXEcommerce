using Modules.Catalog.Domain.Entities;
using Modules.Catalog.Domain.ValueObjects;

namespace Modules.Catalog.Domain.Lookups.Interfaces;

/// <summary>
/// Resolves and validates color codes against the configured catalog (appsettings,
/// Azure App Configuration, etc). The implementation lives in infrastructure —
/// the domain and application only know this interface.
/// </summary>
public interface IColorCatalog
{
	/// <summary>
	/// Write/validation-path lookup. Returns the color value object when the code exists in the
	/// catalog, or <see langword="null"/> when the code is missing, blank or unknown.
	/// This method never throws: callers decide whether an absent code is an error.
	/// </summary>
	/// <param name="code">The color code to resolve (matched case-insensitively).</param>
	/// <returns>The matching <see cref="Color"/>, or <see langword="null"/> when not found.</returns>
	Color? Resolve(string code);

	/// <summary>Gets the display name of a resolved color (for building response DTOs).</summary>
	string GetDisplayName(Color color);

	/// <summary>
	/// Read-path lookup. Gets the display name directly from a stored color code, without
	/// reconstructing the <see cref="Color"/> value object. Used when projecting DTOs straight from
	/// query results (avoids per-row value-object validation during in-memory enrichment).
	/// This method never throws and never validates the code: when the code is absent from the
	/// catalog, the raw stored code is returned as a graceful fallback.
	/// </summary>
	/// <param name="code">The stored color code to look up.</param>
	/// <returns>The configured display name, or the raw <paramref name="code"/> when not found.</returns>
	string GetDisplayName(string code);

	/// <summary>
	/// Read-path enumeration of all configured colors. Never throws.
	/// </summary>
	IReadOnlyCollection<CatalogEntry> GetAll();
}
