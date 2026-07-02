using MediatR;
using Sales.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Features.Orders.CreateOrder
{
    public record OrderItemRequest(
     Guid ProductId,
     string ProductName,
     int Quantity,
     decimal UnitPrice
    );

    public record CreateOrderCommand(
        Guid CustomerId,
        List<OrderItemRequest> Items,
        string? Note = null
    ) : IRequest<Result<Guid>>;
}
