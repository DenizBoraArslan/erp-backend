using MediatR;
using Sales.Application.Common;
using Sales.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Sales.Application.Features.Customers.DeactivateCustomer
{
    public class DeactivateCustomerCommandHandler : IRequestHandler<DeactivateCustomerCommand, Result<bool>>
    {
        private readonly ICustomerRepository _customerRepository;

        public DeactivateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<bool>> Handle(DeactivateCustomerCommand request, CancellationToken ct)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, ct);
            if (customer is null)
                return Result<bool>.Failure("Müşteri bulunamadı.");

            customer.Deactivate();
            await _customerRepository.UpdateAsync(customer, ct);

            return Result<bool>.Success(true);
        }
    }
}
