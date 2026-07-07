namespace Inventory.Application.Consumers;

using erp.Shared.Events;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Interfaces;
using MassTransit;

// Bir Satış Siparişi onaylandığında (Sales modülü) tetiklenir: ilgili
// ürünlerin stoğunu düşer ve izlenebilirlik için bir StockMovement (Out)
// kaydı oluşturur. Stok yetersizse o kalem atlanır (sipariş zaten Sales
// tarafında onaylanmış durumda; burada başarısız olmak o işlemi geri almaz).
public class OrderConfirmedConsumer : IConsumer<OrderConfirmedEvent>
{
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;

    public OrderConfirmedConsumer(IProductRepository productRepository, IStockMovementRepository stockMovementRepository)
    {
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
    }

    public async Task Consume(ConsumeContext<OrderConfirmedEvent> context)
    {
        var evt = context.Message;

        foreach (var item in evt.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, context.CancellationToken);
            if (product is null)
                continue;

            try
            {
                product.RemoveStock(item.Quantity);
            }
            catch (InvalidOperationException)
            {
                // Stok yetersiz — bu ürünü atla, diğerlerini işlemeye devam et.
                continue;
            }

            await _productRepository.UpdateAsync(product, context.CancellationToken);

            var movement = StockMovement.Create(
                item.ProductId,
                item.Quantity,
                MovementType.Out,
                $"Sipariş No: {evt.OrderNumber}");
            await _stockMovementRepository.AddAsync(movement, context.CancellationToken);
        }
    }
}
