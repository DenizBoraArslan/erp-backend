using Finance.Domain.Entites;

namespace Finance.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Invoice>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Invoice>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
        Task<Invoice?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
        Task AddAsync(Invoice invoice, CancellationToken ct = default);
        Task UpdateAsync(Invoice invoice, CancellationToken ct = default);
        Task UpdatePaidAmountAsync(Guid invoiceId, decimal amount, CancellationToken ct = default);
        Task MarkAsPaidAsync(Guid invoiceId, CancellationToken ct = default);
    }
}
