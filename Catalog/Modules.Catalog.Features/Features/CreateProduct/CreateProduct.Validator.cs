using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Modules.Catalog.PublicApi.Contracts;

namespace Modules.Catalog.Features.Features.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
	public CreateProductValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("Product name cannot be empty");

		RuleFor(x => x.SKU)
			.NotEmpty()
			.WithMessage("Product SKU cannot be empty");

		RuleFor(x => x.Price)
			.NotEmpty()
			.WithMessage("Product price cannot be empty")
			.GreaterThan(0)
			.WithMessage("Product price must be a positive value");

		RuleFor(x => x.Description)
			.NotNull()
			.WithMessage("Product description cannot be null");

	}
}
