using FluentValidation.Results;
using Modules.Common.Domain.Results;

namespace Modules.Basket.Features.Features.Shared.Errors;

internal static class ValidationExtensions
{
	internal static List<Error> ToDomainErrors(this ValidationResult validationResult) =>
		validationResult.Errors
			.Select(error => Error.Validation($"Basket.Validation.{error.PropertyName}", error.ErrorMessage))
			.ToList();
}
