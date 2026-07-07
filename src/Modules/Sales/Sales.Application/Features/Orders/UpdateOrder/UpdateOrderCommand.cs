using MediatR;
using Sales.Application.Common;
using System;
using System.Collections.Generic;

namespace Sales.Application.Features.Orders.UpdateOrder
{
    public record UpdateOrderItemRequest(
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice
    );

    public record UpdateOrderCommand(
        Guid OrderId,
        List<UpdateOrderItemRequest> Items,
        string? Note = null
    ) : IRequest<Result<bool>>;
}
