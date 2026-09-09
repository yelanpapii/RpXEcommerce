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
using Modules.Common.Application.Messaging;
using System.Text.Json;
using Modules.Common.Infrastructure.Messaging;

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

		var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Name == request.Name, cancellationToken);
		if (product is not null)
		{
			logger.LogWarning("Product with Name {ProductName} already exists", request.Name);

			return CatalogErrors.AlreadyExists(request.Name);
		}

		var requestProduct = request.MapToProduct();

		dbContext.Products.Add(requestProduct);
		dbContext.OutboxMessages.Add(new OutboxMessage
		{
			Id = Guid.NewGuid(),
			Type = nameof(StockInitializationRequested),
			Payload = JsonSerializer.Serialize(new StockInitializationRequested(
				Guid.NewGuid(),
				requestProduct.Name,
				requestProduct.Variants.Sum(variant => variant.Stock))),
			OccurredOnUtc = DateTimeOffset.UtcNow
		});

		await dbContext.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Created product {ProductId}", requestProduct.Id);

		return Result.Success;
	}
}
