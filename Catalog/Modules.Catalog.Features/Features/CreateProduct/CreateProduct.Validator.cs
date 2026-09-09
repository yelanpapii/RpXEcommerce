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

		RuleFor(x => x.CategoryCode)
			.NotEmpty()
			.Length(3, 4)
			.WithMessage("Product category code must contain between 3 and 4 characters");

		RuleFor(x => x.StyleId)
			.NotEmpty()
			.Must(styleId => int.TryParse(styleId, out _))
			.WithMessage("Product style ID must be a valid number");

		RuleFor(x => x.Description)
			.MaximumLength(1000);

		RuleFor(x => x.ProductVariant)
			.NotEmpty()
			.WithMessage("At least one product variant is required");

		RuleForEach(x => x.ProductVariant)
			.ChildRules(variant =>
			{
				variant.RuleFor(x => x.Size).NotEmpty();
				variant.RuleFor(x => x.ColorName).NotEmpty();
				variant.RuleFor(x => x.ColorCode).NotEmpty().Length(3);
				variant.RuleFor(x => x.Price).GreaterThan(0);
				variant.RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
			});

	}
}
