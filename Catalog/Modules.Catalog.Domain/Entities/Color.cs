using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Catalog.Domain.Entities;

/// <summary>
/// The domain does not validate the code against the catalog — that validation is
/// responsibility of <see cref="IColorCatalog"/> in the application layer, before
/// building this object. The domain only ensures that the code has a valid format.
/// </summary>
public sealed record Color
{
	public string Code { get; }

	private Color(string code) => Code = code;

	/// <summary>
	/// Build the value object by splitting from a normalized code. Only should
	/// be called from <see cref="IColorCatalog"/> (after confirming the code exists
	/// in the configured catalog) — never directly from application code.
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public static Color Of(string code)
	{
		if (string.IsNullOrWhiteSpace(code))
		{
			throw new ArgumentException("The color code is required.", nameof(code));
		}

		return new Color(code.Trim().ToUpperInvariant());
	}

	public override string ToString() => Code;
}

