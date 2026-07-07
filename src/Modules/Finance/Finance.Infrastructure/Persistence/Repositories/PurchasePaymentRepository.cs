using Finance.Domain.Entites;
using Finance.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance.Infrastructure.Persistence.Repositories
{
    public class PurchasePaymentRepository : IPurchasePaymentRepository
    {
        private readonly FinanceDbContext _context;

        public PurchasePaymentRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchasePayment>> GetByPurchaseInvoiceIdAsync(Guid purchaseInvoiceId, CancellationToken ct = default)
            => await _context.PurchasePayments
                .Where(p => p.PurchaseInvoiceId == purchaseInvoiceId)
                .ToListAsync(ct);

        public async Task AddAsync(PurchasePayment payment, CancellationToken ct = default)
        {
            await _context.PurchasePayments.AddAsync(payment, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
