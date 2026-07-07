using MediatR;
using Sales.Application.Common;
using System;

namespace Sales.Application.Features.Customers.DeactivateCustomer
{
    public record DeactivateCustomerCommand(Guid CustomerId) : IRequest<Result<bool>>;
}
