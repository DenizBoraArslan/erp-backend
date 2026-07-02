using MediatR;
using Sales.Application.Common;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Features.Customers.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken ct)
        {
            var exists = await _customerRepository.ExistsByEmailAsync(request.Email, ct);
            if (exists)
                return Result<Guid>.Failure("A customer with this email already exists.");

            var customer = Customer.Create(request.FirstName, request.LastName, request.Email, request.Phone, request.Address);
            await _customerRepository.AddAsync(customer, ct);

            return Result<Guid>.Success(customer.Id);
        }
    }
}
