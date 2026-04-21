using MediatR;
using Common.Results;

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
    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement repository pattern
        await Task.CompletedTask;
        return Result.Failure<CreateProductResponse>("Not implemented yet");
    }
}

public class CreateProductResponse
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
