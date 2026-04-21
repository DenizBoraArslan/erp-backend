using MediatR;
using Common.Results;

namespace Sales.Application.Features.Customers.Commands;

public class CreateCustomerCommand : IRequest<Result<CreateCustomerResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Type { get; set; }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CreateCustomerResponse>>
{
    public async Task<Result<CreateCustomerResponse>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement repository pattern
        await Task.CompletedTask;
        return Result.Failure<CreateCustomerResponse>("Not implemented yet");
    }
}

public class CreateCustomerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
