using FluentValidation;
using Modules.Stocks.PublicApi.Contracts;

namespace Modules.Stocks.Features.Features.GetStocksByProductName;

public class GetStocksByProductNameRequestValidator : AbstractValidator<GetStocksByProductIdRequest>
{
    public GetStocksByProductNameRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEqual(Guid.Empty)
            .WithMessage("Product ID is required");
    }
}
