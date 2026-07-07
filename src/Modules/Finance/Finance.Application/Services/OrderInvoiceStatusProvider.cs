using erp.Shared.Contracts;
using Finance.Domain.Enums;
using Finance.Domain.Interfaces;

namespace Finance.Application.Services
{
    /// <summary>
    /// Finance-side implementation of the shared cross-module read port. Every
    /// order gets a Draft invoice automatically the moment it's created (see
    /// OrderCreatedConsumer), so looking the invoice up by OrderId and mapping
    /// its status is enough to answer "has this order's invoice been paid?".
    /// </summary>
    public class OrderInvoiceStatusProvider : IOrderInvoiceStatusProvider
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public OrderInvoiceStatusProvider(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<OrderInvoicePaymentStatus> GetPaymentStatusAsync(Guid orderId, CancellationToken ct = default)
        {
            var invoice = await _invoiceRepository.GetByOrderIdAsync(orderId, ct);
            if (invoice is null)
                return OrderInvoicePaymentStatus.NoInvoice;

            return invoice.Status switch
            {
                InvoiceStatus.Paid => OrderInvoicePaymentStatus.Paid,
                InvoiceStatus.Cancelled => OrderInvoicePaymentStatus.Cancelled,
                _ => OrderInvoicePaymentStatus.NotPaid,
            };
        }
    }
}
