using Inventory.Application.Common;
using Inventory.Application.DTOs;
using Inventory.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Products.GetProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<IEnumerable<ProductDto>>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<IEnumerable<ProductDto>>> Handle(GetProductsQuery request, CancellationToken ct)
        {
            var products = request.CategoryId.HasValue
                ? await _productRepository.GetByCategoryAsync(request.CategoryId.Value, ct)
                : await _productRepository.GetAllAsync(ct);

            var dtos = products.Select(p => new ProductDto(
                p.Id, p.Name, p.Description, p.SKU, p.Price, p.CostPrice,
                p.StockQuantity, p.MinStockLevel, p.IsLowStock(),
                p.CategoryId, p.Category?.Name ?? string.Empty, p.IsActive,
                p.CreatedAt
            ));

            return Result<IEnumerable<ProductDto>>.Success(dtos);
        }
    }
}
