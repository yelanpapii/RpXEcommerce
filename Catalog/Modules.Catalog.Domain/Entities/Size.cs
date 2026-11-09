using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Catalog.Domain.Entities;

public sealed record Size
{
	public string Code { get; }

	private Size(string code) => Code = code;

	/// <summary>
	/// Build the value object from a normalized code. Only should
	/// be called from <see cref="ISizeCatalog"/> (after confirming the code exists
	/// in the configured catalog) — never directly from application code.
	/// </summary>
	/// <exception cref="ArgumentException"></exception>
	public static Size Of(string code)
	{
		//if (string.IsNullOrWhiteSpace(code))
		//{
		//	throw new ArgumentException("The size code is required.", nameof(code));
		//}

		return new Size(code.Trim().ToUpperInvariant());
	}

	public override string ToString() => Code;
}
