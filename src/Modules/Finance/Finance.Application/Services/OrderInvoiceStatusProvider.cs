using erp.Shared.Contracts;
using Finance.Domain.Enums;
using Finance.Domain.Interfaces;

namespace Finance.Application.Services
{

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
