using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Catalog.Domain.Entities;

/// <summary>
/// Category code for the catalog (ej: "TSHR", "JEAN", "POLO").
/// In this case, the category code is a value object that represents a specific category of products in the catalog. It is used to group products together based on their category and can be used for filtering and searching products in the catalog.
/// </summary>
public sealed record CategoryCode
{
	public string Value { get; }

	private CategoryCode(string value) => Value = value;

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0011:Add Keys", Justification = "<Pending>")]
	public static CategoryCode Create(string value)
	{
		return new CategoryCode(value ?? string.Empty);
	}

	public override string ToString() => Value;

	public static implicit operator string(CategoryCode code) => code.Value;
}
