using FluentValidation;
using Modules.Basket.PublicApi.Contracts;

namespace Modules.Basket.Features.Features.AddBasketItem;

public sealed class AddBasketItemValidator : AbstractValidator<AddBasketItemRequest>
{
	public AddBasketItemValidator()
	{
		RuleFor(request => request.ProductId).NotEmpty();
		RuleFor(request => request.ProductName).NotEmpty().MaximumLength(200);
		RuleFor(request => request.UnitPrice).GreaterThanOrEqualTo(0);
		RuleFor(request => request.Quantity).InclusiveBetween(1, 100);
		RuleFor(request => request.Size).MaximumLength(20);
		RuleFor(request => request.Color).MaximumLength(50);
	}
}
