using erp.Shared.Contracts;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services
{
    public class ProductStockProvider : IProductStockProvider
    {
        private readonly IProductRepository _productRepository;

        public ProductStockProvider(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductStockInfo?> GetStockInfoAsync(Guid productId, CancellationToken ct = default)
        {
            var product = await _productRepository.GetByIdAsync(productId, ct);
            if (product is null)
                return null;

            return new ProductStockInfo(product.StockQuantity, product.MinStockLevel, product.IsActive);
        }
    }
}
