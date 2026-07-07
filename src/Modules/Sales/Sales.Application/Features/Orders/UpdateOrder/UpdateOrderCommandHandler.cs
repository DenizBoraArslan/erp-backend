using MediatR;
using Sales.Application.Common;
using Sales.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sales.Application.Features.Orders.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Result<bool>>
    {
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<bool>> Handle(UpdateOrderCommand request, CancellationToken ct)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);
            if (order is null)
                return Result<bool>.Failure("Sipariş bulunamadı.");

            if (request.Items == null || request.Items.Count == 0)
                return Result<bool>.Failure("Sipariş en az bir ürün içermelidir.");

            try
            {
                order.UpdateDetails(
                    request.Note,
                    request.Items.Select(i => (i.ProductId, i.ProductName, i.Quantity, i.UnitPrice))
                );
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
