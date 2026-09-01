using System;
using System.Collections.Generic;
using System.Text;
using Modules.Common.Domain.Results;

namespace Modules.Catalog.Features.Features.Shared.Errors;

internal static class CatalogErrors
{
	private const string ErrorPrefix = "Catalog";

	internal static Error NotFound(string productName) =>
		Error.NotFound($"{ErrorPrefix}.{nameof(NotFound)}", $"Product with Name {productName} not found");

	internal static Error ValidationError(string propertyName, string errorMessage) =>
		Error.Validation($"{ErrorPrefix}.{nameof(ValidationError)}", $"{propertyName}: {errorMessage}");

	internal static Error AlreadyExists(string productName) =>
		Error.Conflict($"{ErrorPrefix}.{nameof(AlreadyExists)}", $"Product with name {productName} already exists");
}
