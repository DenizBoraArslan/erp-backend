using MediatR;
using Common.Results;

namespace Purchase.Application.Features.Suppliers.Commands;

public class CreateSupplierCommand : IRequest<Result<CreateSupplierResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Type { get; set; }
}

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<CreateSupplierResponse>>
{
    public async Task<Result<CreateSupplierResponse>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement repository pattern
        await Task.CompletedTask;
        return Result.Failure<CreateSupplierResponse>("Not implemented yet");
    }
}

public class CreateSupplierResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
