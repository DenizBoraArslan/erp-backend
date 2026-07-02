using MediatR;
using Sales.Application.Common;
using Sales.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Features.Customers.GetCustomers
{
    public record GetCustomersQuery : IRequest<Result<IEnumerable<CustomerDto>>>;
}
