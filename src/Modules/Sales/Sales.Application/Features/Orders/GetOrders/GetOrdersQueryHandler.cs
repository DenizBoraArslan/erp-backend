using MediatR;
using Sales.Application.Common;
using Sales.Application.DTOs;
using Sales.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Features.Orders.GetOrders
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, Result<IEnumerable<OrderDto>>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<IEnumerable<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken ct)
        {
            var orders = request.CustomerId.HasValue
                ? await _orderRepository.GetByCustomerIdAsync(request.CustomerId.Value, ct)
                : await _orderRepository.GetAllAsync(ct);

            var dtos = orders.Select(o => new OrderDto(
                o.Id,
                o.OrderNumber,
                o.CustomerId,
                $"{o.Customer?.FirstName} {o.Customer?.LastName}",
                o.Status.ToString(),
                o.TotalAmount,
                o.Note,
                o.CreatedAt,
                o.OrderItems.Select(i => new OrderItemDto(
                    i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice
                )).ToList()
            ));

            return Result<IEnumerable<OrderDto>>.Success(dtos);
        }
    }
}
