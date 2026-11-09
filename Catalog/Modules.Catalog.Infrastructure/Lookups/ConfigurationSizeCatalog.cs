using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Modules.Catalog.Domain.Entities;
using Modules.Catalog.Domain.Lookups.Interfaces;
using Modules.Catalog.Domain.Options;
using Modules.Catalog.Domain.ValueObjects;

namespace Modules.Catalog.Infrastructure.Lookups;

/// <summary>
/// Reads the configured size catalog from <see cref="ProductCatalogOptions"/>.
/// Handles both the write path (nullable <see cref="Resolve"/>) and the read path
/// (<see cref="GetDisplayName(string)"/>, which never validates and never throws).
/// </summary>
public sealed class ConfigurationSizeCatalog : ISizeCatalog
{
	private readonly IOptionsMonitor<ProductCatalogOptions> _options;

	/// <summary>
	/// Creates the size catalog backed by the monitored product catalog options.
	/// </summary>
	/// <param name="options">Monitored options holding the configured sizes.</param>
	public ConfigurationSizeCatalog(IOptionsMonitor<ProductCatalogOptions> options)
	{
		_options = options;
	}

	/// <summary>
	/// Write/validation-path lookup. Returns <see langword="null"/> when the code is missing,
	/// blank or unknown. Never throws.
	/// </summary>
	/// <param name="code">The size code to resolve (matched case-insensitively).</param>
	/// <returns>The matching <see cref="Size"/>, or <see langword="null"/> when not found.</returns>
	public Size? Resolve(string code)
	{
		var entry = Find(code);
		return entry is null ? null : Size.Of(entry.Code);
	}

	/// <summary>Gets the display name of a resolved size (for building response DTOs).</summary>
	public string GetDisplayName(Size size) =>
		Find(size.Code)?.Name ?? size.Code;

	/// <summary>
	/// Read-path lookup by stored code. Never validates and never throws: returns the raw
	/// stored code when it is not present in the configured catalog.
	/// </summary>
	/// <param name="code">The stored size code to look up.</param>
	/// <returns>The configured display name, or the raw <paramref name="code"/> when not found.</returns>
	public string GetDisplayName(string code) =>
		Find(code)?.Name ?? code;

	/// <summary>Read-path enumeration of all configured sizes, ordered by sort order.</summary>
	public IReadOnlyCollection<CatalogEntry> GetAll() =>
		_options.CurrentValue.Sizes
			.OrderBy(e => e.SortOrder)
			.Select(e => new CatalogEntry(e.Code, e.Name, e.SortOrder))
			.ToList();

	/// <summary>
	/// Finds a configured size entry by code, matching case-insensitively.
	/// Returns <see langword="null"/> for blank input or when the code is absent.
	/// </summary>
	/// <param name="code">The size code to search for.</param>
	/// <returns>The matching entry, or <see langword="null"/>.</returns>
	private CatalogEntryOptions? Find(string? code)
	{
		if (string.IsNullOrWhiteSpace(code))
		{
			return null;
		}

		var normalized = code.Trim();
		return _options.CurrentValue.Sizes
			.FirstOrDefault(e => string.Equals(e.Code, normalized, StringComparison.OrdinalIgnoreCase));
	}
}
