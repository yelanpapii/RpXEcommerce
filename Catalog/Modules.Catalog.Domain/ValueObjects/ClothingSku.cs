using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Modules.Catalog.Domain.ValueObjects;

public readonly partial record struct ClothingSku
{
	private readonly string _value;
	private static readonly Regex ClothingSkuRegex = ClothingRegex();

	private ClothingSku(string validatedValue)
	{
		_value = validatedValue;
	}

	public ClothingSku(string category, int styleId, string colorCode, string sizeCode)
	{
		var fullSku = $"{category.Trim().ToUpperInvariant()}-{styleId:D4}-{colorCode.Trim().ToUpperInvariant()}-{sizeCode.Trim().ToUpperInvariant()}";

		if (!ClothingSkuRegex.IsMatch(fullSku))
		{
			throw new ArgumentException($"Formato de SKU inválido: '{fullSku}'");
		}
			

		_value = fullSku;
	}

	public static ClothingSku Parse(string value)
	{
		if (string.IsNullOrWhiteSpace(value) || !ClothingSkuRegex.IsMatch(value.ToUpperInvariant()))
		{
			throw new ArgumentException($"El SKU recuperado de la base de datos está corrupto: '{value}'");
		}

		return new ClothingSku(value.ToUpperInvariant());
	}

	public override string ToString() => _value;
	public static implicit operator string(ClothingSku sku) => sku._value;

	[GeneratedRegex(@"^[A-Z]{3,4}-[0-9]{4}-[A-Z]{3}-[A-Z0-9]{1,3}$", RegexOptions.Compiled)]
	private static partial Regex ClothingRegex();
}
