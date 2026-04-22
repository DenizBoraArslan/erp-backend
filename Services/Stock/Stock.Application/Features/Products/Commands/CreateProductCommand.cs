using MediatR;
using Common.Results;
using Common.Interfaces;
using Stock.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Stock.Application.Features.Products.Commands;

public class CreateProductCommand : IRequest<Result<CreateProductResponse>>
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int ReorderLevel { get; set; }
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IRepository<Product> productRepository, IUnitOfWork unitOfWork, ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = new Product
            {
                Sku = request.Sku,
                Name = request.Name,
                Description = request.Description,
                Quantity = request.Quantity,
                ReorderLevel = request.ReorderLevel,
                UnitPrice = request.UnitPrice,
                Unit = request.Unit,
                CategoryId = request.CategoryId,
                IsActive = true
            };

            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new CreateProductResponse
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            var message = ex.InnerException?.Message ?? ex.Message;
            throw;
        }
    }
}

public class CreateProductResponse
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
