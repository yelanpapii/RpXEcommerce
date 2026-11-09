using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Features.Features.Shared.Converters;
using Modules.Catalog.Features.Features.Shared.Errors;
using Modules.Catalog.Infrastructure.Database;
using Modules.Catalog.PublicApi.Contracts;
using Modules.Common.Application.Messaging.Stocks;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Common.Infrastructure.Messaging;

namespace Modules.Catalog.Features.Features.CreateProduct;

internal interface ICreateProductHandler : IHandler
{
	Task<Result<Success>> HandleAsync(CreateProductRequest request, CancellationToken cancellationToken);
}

internal sealed class CreateProductHandler(CatalogDbContext dbContext,
	IValidator<CreateProductRequest> validator,
	ILogger<CreateProductHandler> logger,
	ProductMapper productMapper) : ICreateProductHandler
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

		var mappingResult = productMapper.MapToProduct(request);
		if (mappingResult.IsError)
		{
			return mappingResult.Errors;
		}

		var requestProduct = mappingResult.Value!;
		dbContext.Products.Add(requestProduct);

		var messageId = Guid.CreateVersion7();
		var requestedVariants = requestProduct.Variants
			.ToDictionary(
				variant => variant.Sku.ToString(),
				variant => variant.Stock.ToString(CultureInfo.InvariantCulture));

		dbContext.OutboxMessages.Add(new OutboxMessage
		{
			Id = messageId,
			Type = nameof(StockInitializationRequested),
			Payload = JsonSerializer.Serialize(new StockInitializationRequested(
				messageId,
				requestProduct.Id,
				requestProduct.Name,
				requestedVariants,
				requestProduct.Variants.Sum(v => v.Stock))),
			OccurredOnUtc = DateTimeOffset.UtcNow
		});

		await dbContext.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Created product {ProductId}", requestProduct.Id);

		return Result.Success;
	}
}
