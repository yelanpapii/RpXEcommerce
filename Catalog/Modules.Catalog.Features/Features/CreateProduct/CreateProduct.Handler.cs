using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Features.Features.Shared.Errors;
using Modules.Catalog.Infrastructure.Database;
using Modules.Catalog.PublicApi.Contracts;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;

namespace Modules.Catalog.Features.Features.CreateProduct;

internal interface ICreateProductHandler : IHandler
{
	Task<Result<Success>> HandleAsync(CreateProductRequest request, CancellationToken cancellationToken);
}

internal sealed class CreateProductHandler(CatalogDbContext dbContext,
	IValidator<CreateProductRequest> validator,
	ILogger<CreateProductHandler> logger) : ICreateProductHandler
{
	public async Task<Result<Success>> HandleAsync(CreateProductRequest request, CancellationToken cancellationToken)
	{
		logger.LogInformation("Creating product {ProductName}", request.Name);

		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return validationResult.ToDomainErrors();
		}

		var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Name == request.Name && x.SKU == request.SKU, cancellationToken);
		if (product is not null)
		{
			logger.LogWarning("Product with Name {ProductName} and SKU {SKU} already exists", request.Name, request.SKU);

			return CatalogErrors.AlreadyExists(request.Name);
		}

		var requestProduct = request.MapToProduct();

		dbContext.Products.Add(requestProduct);
		await dbContext.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Created product {ProductId}", requestProduct.Id);

		return Result.Success;
	}
}
