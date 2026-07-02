using MediatR;
using Sales.Application.Common;
using Sales.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Features.Orders.GetOrders
{
    public record GetOrdersQuery(Guid? CustomerId = null) : IRequest<Result<IEnumerable<OrderDto>>>;
}
