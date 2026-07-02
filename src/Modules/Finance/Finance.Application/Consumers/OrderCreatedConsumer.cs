namespace Finance.Application.Consumers;

using erp.Shared.Events;
using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using MassTransit;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public OrderCreatedConsumer(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var evt = context.Message;

        var invoice = Invoice.Create(
            evt.CustomerId,
            evt.CustomerName,
            null,  // vade tarihi boş
            evt.OrderId
        );

        foreach (var item in evt.Items)
            invoice.AddItem(item.ProductName, item.Quantity, item.UnitPrice);

        await _invoiceRepository.AddAsync(invoice, context.CancellationToken);
    }
}