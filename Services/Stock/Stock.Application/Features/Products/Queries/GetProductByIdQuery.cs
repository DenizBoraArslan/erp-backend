using MediatR;
using Common.Results;
using Stock.Domain.Entities;

namespace Stock.Application.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<Result<GetProductByIdResponse>>
{
    public int ProductId { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<GetProductByIdResponse>>
{
    public async Task<Result<GetProductByIdResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement repository pattern
        await Task.CompletedTask;
        return Result.Failure<GetProductByIdResponse>("Not implemented yet");
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
