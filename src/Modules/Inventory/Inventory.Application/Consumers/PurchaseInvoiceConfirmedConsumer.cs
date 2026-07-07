namespace Inventory.Application.Consumers;

using erp.Shared.Events;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Interfaces;
using MassTransit;

// Bir Alım Faturası onaylandığında (Finance modülü) tetiklenir: ürüne
// bağlanmış kalemler için stoğu artırır ve izlenebilirlik için bir
// StockMovement (In) kaydı oluşturur.
public class PurchaseInvoiceConfirmedConsumer : IConsumer<PurchaseInvoiceConfirmedEvent>
{
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;

    public PurchaseInvoiceConfirmedConsumer(IProductRepository productRepository, IStockMovementRepository stockMovementRepository)
    {
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
    }

    public async Task Consume(ConsumeContext<PurchaseInvoiceConfirmedEvent> context)
    {
        var evt = context.Message;

        foreach (var item in evt.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, context.CancellationToken);
            if (product is null)
                continue;

            product.AddStock(item.Quantity);
            await _productRepository.UpdateAsync(product, context.CancellationToken);

            var movement = StockMovement.Create(
                item.ProductId,
                item.Quantity,
                MovementType.In,
                $"Alım Faturası No: {evt.InvoiceNumber}");
            await _stockMovementRepository.AddAsync(movement, context.CancellationToken);
        }
    }
}
