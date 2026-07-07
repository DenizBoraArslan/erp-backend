using Inventory.Application.Common;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Products.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, ct);
            if (category is null)
                return Result<Guid>.Failure("Category not found.");

            var product = Product.Create(
                request.Name,
                request.Price,
                request.CategoryId,
                request.Description,
                request.MinStockLevel,
                request.CostPrice
            );

            await _productRepository.AddAsync(product, ct);
            return Result<Guid>.Success(product.Id);
        }
    }
}
