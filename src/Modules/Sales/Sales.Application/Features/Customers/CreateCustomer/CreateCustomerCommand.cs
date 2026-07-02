using MediatR;
using Sales.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Features.Customers.CreateCustomer
{
    public record CreateCustomerCommand(
     string FirstName,
     string LastName,
     string Email,
     string? Phone = null,
     string? Address = null
 ) : IRequest<Result<Guid>>;
}
