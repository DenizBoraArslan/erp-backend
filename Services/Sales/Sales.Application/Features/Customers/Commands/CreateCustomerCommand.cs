using MediatR;
using Common.Results;
using Common.Interfaces;
using Sales.Domain.Entities;

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
    private readonly IRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(IRepository<Customer> customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCustomerResponse>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = new Customer
            {
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                City = request.City,
                Country = request.Country,
                Type = (CustomerType)request.Type,
                CreditLimit = 0,
                IsActive = true
            };

            await _customerRepository.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new CreateCustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name
            };

            return Result.Success(response);
        }
        catch (Exception)
        {
            return Result.Failure<CreateCustomerResponse>("Unexpected error occurred while creating customer.");
        }
    }
}

public class CreateCustomerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
