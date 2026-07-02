using Finance.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken ct = default);
        Task AddAsync(Payment payment, CancellationToken ct = default);
    }
}
