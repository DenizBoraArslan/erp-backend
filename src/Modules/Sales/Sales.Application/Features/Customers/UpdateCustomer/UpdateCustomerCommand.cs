using MediatR;
using Sales.Application.Common;
using System;

namespace Sales.Application.Features.Customers.UpdateCustomer
{
    public record UpdateCustomerCommand(
     Guid Id,
     string FirstName,
     string LastName,
     string? Phone = null,
     string? Address = null
 ) : IRequest<Result<Guid>>;
}
