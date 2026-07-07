using Finance.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Domain.Interfaces
{
    public interface IPurchaseInvoiceRepository
    {
        Task<PurchaseInvoice?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<PurchaseInvoice>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<PurchaseInvoice>> GetBySupplierIdAsync(Guid supplierId, CancellationToken ct = default);
        Task AddAsync(PurchaseInvoice purchaseInvoice, CancellationToken ct = default);
        Task UpdateAsync(PurchaseInvoice purchaseInvoice, CancellationToken ct = default);
        Task UpdatePaidAmountAsync(Guid purchaseInvoiceId, decimal amount, CancellationToken ct = default);
        Task MarkAsPaidAsync(Guid purchaseInvoiceId, CancellationToken ct = default);
    }
}
