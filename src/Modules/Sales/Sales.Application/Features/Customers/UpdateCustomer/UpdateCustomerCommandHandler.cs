using MediatR;
using Sales.Application.Common;
using Sales.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sales.Application.Features.Customers.UpdateCustomer
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<Guid>>
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<Guid>> Handle(UpdateCustomerCommand request, CancellationToken ct)
        {
            var customer = await _customerRepository.GetByIdAsync(request.Id, ct);
            if (customer is null)
                return Result<Guid>.Failure("Müşteri bulunamadı.");

            customer.Update(request.FirstName, request.LastName, request.Phone, request.Address);
            await _customerRepository.UpdateAsync(customer, ct);

            return Result<Guid>.Success(customer.Id);
        }
    }
}
