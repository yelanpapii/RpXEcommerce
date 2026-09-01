using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.Results;
using Modules.Common.Domain.Results;

namespace Modules.Catalog.Features.Features.Shared.Errors;

internal static class ValidationExtensions
{
	internal static List<Error> ToDomainErrors(this ValidationResult validationResult)
	{
		return validationResult.Errors.Select(x => CatalogErrors.ValidationError(x.PropertyName, x.ErrorMessage)).ToList();
	}
}
