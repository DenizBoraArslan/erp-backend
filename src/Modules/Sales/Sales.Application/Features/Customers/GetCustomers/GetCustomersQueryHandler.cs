using MediatR;
using Sales.Application.Common;
using Sales.Application.DTOs;
using Sales.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Features.Customers.GetCustomers
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<IEnumerable<CustomerDto>>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomersQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<IEnumerable<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken ct)
        {
            var customers = await _customerRepository.GetAllAsync(ct);

            var dtos = customers.Select(c => new CustomerDto(
                c.Id, c.FirstName, c.LastName, c.Email, c.Phone, c.Address, c.IsActive
            ));

            return Result<IEnumerable<CustomerDto>>.Success(dtos);
        }
    }
}
