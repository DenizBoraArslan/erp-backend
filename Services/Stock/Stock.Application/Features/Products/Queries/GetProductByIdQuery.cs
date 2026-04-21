using MediatR;
using Common.Results;
using Common.Interfaces;
using Stock.Domain.Entities;

namespace Stock.Application.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<Result<GetProductByIdResponse>>
{
    public int ProductId { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<GetProductByIdResponse>>
{
    private readonly IRepository<Product> _productRepository;

    public GetProductByIdQueryHandler(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<GetProductByIdResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product is null)
                return Result.Failure<GetProductByIdResponse>("Product not found");

            var response = new GetProductByIdResponse
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Quantity = product.Quantity,
                UnitPrice = product.UnitPrice
            };

            return Result.Success(response);
        }
        catch (Exception)
        {
            return Result.Failure<GetProductByIdResponse>("Unexpected error occurred while getting product.");
        }
    }
}

public class GetProductByIdResponse
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
