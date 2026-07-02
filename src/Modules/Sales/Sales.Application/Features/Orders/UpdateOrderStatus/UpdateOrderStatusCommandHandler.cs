using MediatR;
using Sales.Application.Common;
using Sales.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Features.Orders.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result<bool>>
    {
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<bool>> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);
            if (order is null)
                return Result<bool>.Failure("Order not found.");

            try
            {
                switch (request.Action.ToLower())
                {
                    case "confirm": order.Confirm(); break;
                    case "ship": order.Ship(); break;
                    case "deliver": order.Deliver(); break;
                    case "cancel": order.Cancel(); break;
                    default: return Result<bool>.Failure("Invalid action. Use: confirm, ship, deliver, cancel.");
                }
            }
            catch (InvalidOperationException ex)
            {
                return Result<bool>.Failure(ex.Message);
            }

            await _orderRepository.UpdateAsync(order, ct);
            return Result<bool>.Success(true);
        }
    }
}
