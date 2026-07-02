using Inventory.Application.Common;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Features.Products.AddStock
{
    public class AddStockCommandHandler : IRequestHandler<AddStockCommand, Result<bool>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockMovementRepository _stockMovementRepository;

        public AddStockCommandHandler(IProductRepository productRepository, IStockMovementRepository stockMovementRepository)
        {
            _productRepository = productRepository;
            _stockMovementRepository = stockMovementRepository;
        }

        public async Task<Result<bool>> Handle(AddStockCommand request, CancellationToken ct)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
            if (product is null)
                return Result<bool>.Failure("Product not found.");

            product.AddStock(request.Quantity);
            await _productRepository.UpdateAsync(product, ct);

            var movement = StockMovement.Create(request.ProductId, request.Quantity, MovementType.In, request.Note);
            await _stockMovementRepository.AddAsync(movement, ct);

            return Result<bool>.Success(true);
        }
    }
}
