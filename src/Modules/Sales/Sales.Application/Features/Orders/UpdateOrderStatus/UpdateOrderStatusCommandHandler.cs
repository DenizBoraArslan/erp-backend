using erp.Shared.Contracts;
using erp.Shared.Events;
using MassTransit;
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
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IOrderInvoiceStatusProvider _orderInvoiceStatusProvider;
        private readonly IProductStockProvider _productStockProvider;

        public UpdateOrderStatusCommandHandler(
            IOrderRepository orderRepository,
            IPublishEndpoint publishEndpoint,
            IOrderInvoiceStatusProvider orderInvoiceStatusProvider,
            IProductStockProvider productStockProvider)
        {
            _orderRepository = orderRepository;
            _publishEndpoint = publishEndpoint;
            _orderInvoiceStatusProvider = orderInvoiceStatusProvider;
            _productStockProvider = productStockProvider;
        }

        public async Task<Result<bool>> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);
            if (order is null)
                return Result<bool>.Failure("Order not found.");

            var action = request.Action.ToLower();

            // Fatura ödenmeden sipariş teslim edilemez: Finance modülü her
            // sipariş oluşturulduğunda otomatik bir Taslak fatura açıyor
            // (bkz. OrderCreatedConsumer), bu yüzden burada o faturanın
            // ödenip ödenmediğini kontrol edip teslim işlemini engelliyoruz.
            if (action == "deliver")
            {
                var paymentStatus = await _orderInvoiceStatusProvider.GetPaymentStatusAsync(order.Id, ct);
                if (paymentStatus != OrderInvoicePaymentStatus.Paid)
                {
                    return Result<bool>.Failure("Fatura ödenmeden sipariş teslim edilemez.");
                }
            }

            // Stok yeniden kontrolü: sipariş oluşturulduktan sonra başka
            // siparişler stoğu düşürmüş olabilir, bu yüzden gerçek düşümü
            // tetikleyen onay (confirm) anında stok tekrar doğrulanır.
            if (action == "confirm")
            {
                foreach (var item in order.OrderItems)
                {
                    var stockInfo = await _productStockProvider.GetStockInfoAsync(item.ProductId, ct);
                    if (stockInfo is null)
                        return Result<bool>.Failure($"'{item.ProductName}' ürünü bulunamadı.");

                    var remaining = stockInfo.StockQuantity - item.Quantity;
                    if (remaining < stockInfo.MinStockLevel)
                    {
                        return Result<bool>.Failure(
                            $"'{item.ProductName}' için stok yetersiz. Mevcut stok: {stockInfo.StockQuantity}, " +
                            $"istenen: {item.Quantity}, minimum stok seviyesi: {stockInfo.MinStockLevel}.");
                    }
                }
            }

            try
            {
                switch (action)
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

            // Sipariş onaylandığında ilgili ürünlerin stoğunu düşmek ve bu
            // hareketi izlenebilir kılmak için Inventory modülüne bildir.
            if (action == "confirm")
            {
                await _publishEndpoint.Publish(new OrderConfirmedEvent
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    Items = order.OrderItems.Select(i => new OrderConfirmedEventItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                }, ct);
            }

            return Result<bool>.Success(true);
        }
    }
}
