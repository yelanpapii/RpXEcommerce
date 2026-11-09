using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Catalog.Domain.Entities;

/// <summary>
/// Short color code used in the SKU (e.g., "BLC", "AZM").
/// Immutable value object, always normalized to uppercase.
/// </summary>
public sealed record ColorCode
{
	public string Value { get; }

	private ColorCode(string value) => Value = value;

	public static ColorCode Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException("The color code is required.", nameof(value));
		}

		var normalized = value.Trim().ToUpperInvariant();

		if (normalized.Length is < 2 or > 6)
		{
			throw new ArgumentException("The color code must have between 2 and 6 characters.", nameof(value));
		}

		if (!normalized.All(char.IsLetterOrDigit))
		{
			throw new ArgumentException("The color code can only contain letters or digits.", nameof(value));
		}

		return new ColorCode(normalized);
	}

	public override string ToString() => Value;

	public static implicit operator string(ColorCode code) => code.Value;
}
