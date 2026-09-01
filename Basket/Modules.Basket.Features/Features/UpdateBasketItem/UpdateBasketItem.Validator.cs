using FluentValidation;
using Modules.Basket.PublicApi.Contracts;

namespace Modules.Basket.Features.Features.UpdateBasketItem;

public sealed class UpdateBasketItemValidator : AbstractValidator<UpdateBasketItemRequest>
{
	public UpdateBasketItemValidator()
	{
		RuleFor(request => request.ProductId).NotEmpty();
		RuleFor(request => request.Quantity).InclusiveBetween(1, 100);
		RuleFor(request => request.Size).MaximumLength(20);
		RuleFor(request => request.Color).MaximumLength(50);
	}
}
