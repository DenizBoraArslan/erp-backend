using Finance.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Domain.Interfaces
{
    public interface IPurchasePaymentRepository
    {
        Task<IEnumerable<PurchasePayment>> GetByPurchaseInvoiceIdAsync(Guid purchaseInvoiceId, CancellationToken ct = default);
        Task AddAsync(PurchasePayment payment, CancellationToken ct = default);
    }
}
