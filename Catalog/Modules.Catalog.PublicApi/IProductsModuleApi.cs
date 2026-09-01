using Modules.Common.Domain.Results;
using Modules.Catalog.PublicApi.Contracts;

namespace Modules.Catalog.PublicApi;

public interface IProductsModuleApi
{
	Task<Result<List<ProductDto>>> GetProductsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

	Task<Result<ProductDto>> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default);

	Task<Result<Success>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
}
